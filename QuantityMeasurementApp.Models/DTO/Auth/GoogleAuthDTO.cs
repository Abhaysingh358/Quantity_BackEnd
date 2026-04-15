namespace QuantityMeasurementApp.Models.DTO.Auth
{
    // this is the only thing the client sends for Google login
    // the IdToken comes from Google Sign-In on the frontend
    public class GoogleAuthDTO
    {
        public string IdToken { get; set; } = string.Empty;
    }
}