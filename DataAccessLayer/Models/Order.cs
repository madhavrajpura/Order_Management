using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;

[Table("Order", Schema = "public")]
public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [Column("CustomerName")]
    [StringLength(100)]
    public string CustomerName { get; set; }

    [Required]
    [Column("OrderAmount", TypeName = "decimal(10,2)")]
    public decimal OrderAmount { get; set; }

    [Required]
    [ForeignKey("Item")]
    [Column("ItemId")]
    public int ItemId { get; set; }

    [Required]
    [Column("ItemName")]
    [StringLength(100)]
    public string ItemName { get; set; }

    [Required]
    [Column("Quantity")]
    public int Quantity { get; set; }

    [Required]
    [Column("OrderDate")]
    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Required]
    [Column("DeliveryDate")]
    public DateTime DeliveryDate { get; set; }

    [Required]
    [Column("Price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Column("IsDelete")]
    public bool IsDelete { get; set; } = false;

    [Required]
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Column("DeletedAt")]
    public DateTime? DeletedAt { get; set; }

    [Required]
    [Column("CreatedBy")]
    public int CreatedBy { get; set; }

    [Column("UpdatedBy")]
    public int? UpdatedBy { get; set; }

    [Column("DeletedBy")]
    public int? DeletedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("UpdatedBy")]
    public virtual User? UpdatedByUser { get; set; }

    [ForeignKey("DeletedBy")]
    public virtual User? DeletedByUser { get; set; }
    public virtual Item Items { get; set; } = null!;
}
