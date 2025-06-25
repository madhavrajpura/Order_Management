using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.ViewModels;

public class ForgotPasswordViewModel
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required.")]
    [RegularExpression(@"^[a-z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address")]
    [MaxLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
    public string Email { get; set; } = null!;
}