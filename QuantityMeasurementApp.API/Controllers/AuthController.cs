using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.Business.Interfaces;
using QuantityMeasurementApp.Models.DTO.Auth;
using QuantityMeasurementApp.Models.Exceptions;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        try
        {
            var result = await _authService.Register(dto);
            return Ok(new { Message = result });
        }
        catch (AuthException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        try
        {
            var token = await _authService.Login(dto);
            return Ok(new { Token = token });
        }
        catch (AuthException ex)
        {
            return Unauthorized(new { Error = ex.Message });
        }
    }

    // client sends the Google IdToken obtained from Google Sign-In SDK
    // we validate it, find/create user, return our own JWT
    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin(GoogleAuthDTO dto)
    {
        try
        {
            var token = await _authService.GoogleLogin(dto);
            return Ok(new { Token = token });
        }
        catch (AuthException ex)
        {
            return Unauthorized(new { Error = ex.Message });
        }
    }

    // protected endpoint — verify JWT middleware works
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var email      = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var authMethod = User.FindFirst("auth_method")?.Value;
        return Ok(new { Email = email, AuthMethod = authMethod });
    }
}