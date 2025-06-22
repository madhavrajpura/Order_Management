using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace DataAccessLayer.ViewModels;

// Custom validation attribute for ItemName
public class ValidItemNameAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("Item Name is required");
        }

        string input = value.ToString().Trim();

        // Check if string is empty after trimming
        if (string.IsNullOrEmpty(input))
        {
            return new ValidationResult("Item Name cannot be empty or contain only spaces");
        }

        // Check if string contains only special characters
        if (Regex.IsMatch(input, @"^[^a-zA-Z0-9]+$"))
        {
            return new ValidationResult("Item Name cannot contain only special characters");
        }

        // Check if string has at least one alphanumeric character
        if (!Regex.IsMatch(input, @"[a-zA-Z0-9]"))
        {
            return new ValidationResult("Item Name must contain at least one letter or number");
        }

        return ValidationResult.Success;
    }
}

public class ItemViewModel
{
    public int ItemId { get; set; }

    [Required(ErrorMessage = "Item Name is required")]
    [StringLength(100, ErrorMessage = "Item Name cannot exceed 100 characters")]
    [ValidItemName(ErrorMessage = "Invalid Item Name format")]
    public string ItemName { get; set; } = null!;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal Price { get; set; }

    public string? Details { get; set; }

    [Required(ErrorMessage = "Thumbnail image is required")]
    public string? ThumbnailImageUrl { get; set; } = null!;

    [Required(ErrorMessage = "Thumbnail image is required")]
    public IFormFile? ThumbnailImageFile { get; set; } = null!;

    public List<string>? AdditionalImagesUrl { get; set; } = new List<string>();

    [Required(ErrorMessage = "You can upload at most 5 additional images")]
    public List<IFormFile>? AdditionalImagesFile { get; set; } = new List<IFormFile>();

    public DateTime CreatedAt { get; set; }
}