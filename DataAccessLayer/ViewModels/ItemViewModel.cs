using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DataAccessLayer.ViewModels;

public class ItemViewModel
{
    public int ItemId { get; set; }

    [Required(ErrorMessage = "Item Name is required")]
    [StringLength(100, ErrorMessage = "Item Name cannot exceed 100 characters.")]
    public string ItemName { get; set; } = null!;

    [Required(ErrorMessage = " Price is required")]
    [Range(0.01, 99999999.99, ErrorMessage = "Price must be greater than zero")] // Change here
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Details is required")]
    [StringLength(400, ErrorMessage = "Details cannot exceed 400 characters.")]
    public string? Details { get; set; }
    public string? ThumbnailImageUrl { get; set; } = null!;
    public IFormFile? ThumbnailImageFile { get; set; } = null!;
    public List<string>? AdditionalImagesUrl { get; set; } = new List<string>();
    public List<IFormFile>? AdditionalImagesFile { get; set; } = new List<IFormFile>();
    public DateTime CreatedAt { get; set; }
}