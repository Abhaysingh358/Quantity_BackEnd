using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementApp.Business.Interfaces;
using QuantityMeasurementApp.Models.DTO.Auth;
using QuantityMeasurementApp.Models.Entities;
using QuantityMeasurementApp.Models.Exceptions;
using QuantityMeasurementApp.Repositories.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuantityMeasurementApp.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly string _googleClientId;

        // validate config eagerly — fail fast instead of getting
        // a cryptic NullReferenceException deep inside GenerateJwtToken
        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context        = context;
            _jwtKey         = config["Jwt:Key"]          ?? throw new InvalidOperationException("Jwt:Key is missing from config");
            _jwtIssuer      = config["Jwt:Issuer"]       ?? throw new InvalidOperationException("Jwt:Issuer is missing from config");
            _jwtAudience    = config["Jwt:Audience"]     ?? throw new InvalidOperationException("Jwt:Audience is missing from config");
            _googleClientId = config["Google:ClientId"]  ?? throw new InvalidOperationException("Google:ClientId is missing from config");
        }

        // REGISTER — email/password

        public async Task<string> Register(RegisterDTO dto)
        {
            // check for duplicate email before inserting
            bool emailTaken = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailTaken)
                throw new AuthException("Email is already registered");

            var hasher = new PasswordHasher<User>();
            var user = new User { Email = dto.Email };
            user.PasswordHash = hasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "User registered successfully";
        }

        // LOGIN — email/password

        public async Task<string> Login(LoginDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            // intentionally vague — don't reveal whether email exists
            if (user == null)
                throw new AuthException("Invalid email or password");

            // Google-registered users have no password hash
            if (string.IsNullOrEmpty(user.PasswordHash))
                throw new AuthException("This account uses Google Sign-In. Please login with Google.");

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
                throw new AuthException("Invalid email or password");

            return GenerateJwtToken(user);
        }

        // GOOGLE LOGIN

        public async Task<string> GoogleLogin(GoogleAuthDTO dto)
        {
            // step 1 — validate IdToken with Google's servers
            // throws if token is tampered, expired, or for a different app
            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleClientId }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, settings);
            }
            catch (InvalidJwtException ex)
            {
                throw new AuthException($"Invalid Google token: {ex.Message}");
            }

            // step 2 — find or create user by GoogleId
            // using GoogleId not Email — same email could exist as password user
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.GoogleId == payload.Subject);

            if (user == null)
            {
                // auto-register on first Google login
                user = new User
                {
                    Email    = payload.Email,
                    GoogleId = payload.Subject  // stable unique ID from Google
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // step 3 — issue our own JWT, not Google's token
            return GenerateJwtToken(user);
        }

        // SHARED — JWT generation

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                // tells you in controllers whether user used Google or password
                new Claim("auth_method", user.GoogleId != null ? "google" : "password")
            };

            var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:             _jwtIssuer,
                audience:           _jwtAudience,
                claims:             claims,
                expires:            DateTime.UtcNow.AddHours(1),  // UTC not local
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}