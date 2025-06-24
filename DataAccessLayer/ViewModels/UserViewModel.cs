using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DataAccessLayer.ViewModels;

public class UserViewModel
{
    public int UserId { get; set; }
    public int RoleId { get; set; }


    [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be of 10 digits only.")]
    public long PhoneNumber { get; set; }

    public string? Address { get; set; }

    public IFormFile? ImageFile { get; set; }

    public string? ImageURL { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required.")]
    [RegularExpression(@"^[a-z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address")]
    [MaxLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
    public string Email { get; set; } = null!;


    [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    [MaxLength(20, ErrorMessage = "Password cannot exceed 20 characters.")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$", ErrorMessage = "Password must contain at least one uppercase letter, one number, and one special character.")]
    public string Password { get; set; } = null!;


    [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm Password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = null!;

}