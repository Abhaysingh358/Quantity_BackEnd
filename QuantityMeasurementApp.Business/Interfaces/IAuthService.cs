using QuantityMeasurementApp.Models.DTO.Auth;

namespace QuantityMeasurementApp.Business.Interfaces
{
    public interface IAuthService
    {
        Task<string> Register(RegisterDTO dto);
        Task<string> Login(LoginDTO dto);

        // client sends Google IdToken, we validate it and return our own JWT
        Task<string> GoogleLogin(GoogleAuthDTO dto);
    }
}