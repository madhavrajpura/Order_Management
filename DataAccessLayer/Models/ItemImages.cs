using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;

[Table("ItemImages", Schema = "public")]
public class ItemImages
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [ForeignKey("Item")]
    [Column("ItemId")]
    public int ItemId { get; set; }

    [Required]
    [Column("ImageURL")]
    public string ImageURL { get; set; } = null!;

    [Required]
    [Column("IsThumbnail")]

    public bool IsThumbnail { get; set; } = false;

    [Required]
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Required]
    [Column("CreatedBy")]
    public int CreatedBy { get; set; }

    [Column("UpdatedBy")]
    public int? UpdatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    public virtual User? UpdatedByUser { get; set; }
    public virtual Item Items { get; set; } = null!;
}