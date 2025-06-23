using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DataAccessLayer.ViewModels;

public class ItemViewModel
{
    public int ItemId { get; set; }

    [Required(ErrorMessage = "Item Name is required")]
    [StringLength(100, ErrorMessage = "Item Name cannot exceed 100 characters.")]
    public string ItemName { get; set; } = null!;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    public string? Details { get; set; }

    [Required(ErrorMessage = "Thumbnail image is required.")]
    public string? ThumbnailImageUrl { get; set; } = null!;

    [Required(ErrorMessage = "Thumbnail image is required.")]
    public IFormFile? ThumbnailImageFile { get; set; } = null!;
    public List<string>? AdditionalImagesUrl { get; set; } = new List<string>();

    [Required(ErrorMessage = "You can upload atmost 5 additional images.")]
    public List<IFormFile>? AdditionalImagesFile { get; set; } = new List<IFormFile>();

    public DateTime CreatedAt { get; set; }

}