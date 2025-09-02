using System.ComponentModel.DataAnnotations;

namespace CBTApplication.Models
{
    public class VerifyCodeViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Verification code is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be 6 digits.")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "Code must be 6 digits.")]
        [Display(Name = "Verification Code")]
        public string EnteredCode { get; set; } = string.Empty;

        public string? GeneratedCodeForDisplay { get; set; }
    }
}
