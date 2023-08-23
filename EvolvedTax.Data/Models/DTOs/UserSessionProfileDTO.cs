
namespace EvolvedTax.Data.Models.DTOs
{
    public class UserSessionProfileDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public int InstituteId { get; set; }
    }
}
