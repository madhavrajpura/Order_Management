using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;

[Table("Item", Schema = "public")]
public class Item
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [Column("Name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [Column("Price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Column("IsDelete")]
    public bool IsDelete { get; set; } = false;

    [Required]
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Column("DeletedAt")]
    public DateTime? DeletedAt { get; set; }

    [Required]
    // [ForeignKey("Id")]
    [Column("CreatedBy")]
    public int CreatedBy { get; set; }

    // [ForeignKey("User")]
    [Column("UpdatedBy")]
    public int? UpdatedBy { get; set; }

    // [ForeignKey("User")]
    [Column("DeletedBy")]
    public int? DeletedBy { get; set; }
    // public virtual User? Users { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; }

    [ForeignKey("UpdatedBy")]
    public virtual User? UpdatedByUser { get; set; }

    [ForeignKey("DeletedBy")]
    public virtual User? DeletedByUser { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();

}
