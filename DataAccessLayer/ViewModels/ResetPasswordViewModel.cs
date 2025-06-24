using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.ViewModels;

public class ResetPasswordViewModel
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required.")]
    [RegularExpression(@"^[a-z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address")]
    [MaxLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must contains at least 8 characters")]
    [MaxLength(20, ErrorMessage = "Password should not exceed 20 characters")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$", ErrorMessage = "Password must contain at least one uppercase letter, one number,and one special character.")]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
}
