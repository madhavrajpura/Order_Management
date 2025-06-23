using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.ViewModels;

public class ChangePasswordViewModel
{
    public string CurrentPassword { get; set; }

    [MinLength(8,ErrorMessage ="Password must contains at least 8 characters")]
    [MaxLength(20,ErrorMessage ="Password should not exceed 20 characters")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$",ErrorMessage ="Password must contain at least one uppercase letter, one number,and one special character.")]
    public string NewPassword { get; set; }

    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string NewConfirmPassword { get; set; }
}
