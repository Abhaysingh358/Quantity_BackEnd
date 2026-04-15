namespace QuantityMeasurementApp.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }  // nullable — Google users have no password

        // Google's stable unique identifier (payload.Subject)
        // null for email/password users
        public string? GoogleId { get; set; }
    }
}