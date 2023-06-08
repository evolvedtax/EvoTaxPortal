using System.ComponentModel.DataAnnotations;

namespace EvolvedTax.Data.Models.DTOs.Request
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Please enter your user name")]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your password")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
