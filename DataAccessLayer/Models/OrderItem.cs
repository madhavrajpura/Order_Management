using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;


[Table("OrderItem", Schema = "public")]
public class OrderItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Id")]
    public int Id { get; set; }

    [Column("OrderId")]
    public int? OrderId { get; set; }

    [Required]
    [Column("ItemId")]
    public int ItemId { get; set; }

    [Required]
    [Column("ItemName")]
    public string ItemName { get; set; } = null!;

    [Required]
    [Column("Price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Required]
    [Column("Quantity")]
    public int Quantity { get; set; }

    [Column("IsDelete")]
    [DefaultValue("false")]
    public bool IsDelete { get; set; } = false;

    [Column("CreatedAt", TypeName = "timestamp without time zone")]
    [DefaultValue("now()")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("UpdatedAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("DeletedAt", TypeName = "timestamp without time zone")]
    public DateTime? DeletedAt { get; set; }

    [Required]
    [Column("CreatedBy")]
    public int CreatedBy { get; set; }

    [Column("UpdatedBy")]
    public int? UpdatedBy { get; set; }

    [Column("DeletedBy")]
    public int? DeletedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    public virtual User? UpdatedByUser { get; set; }

    [ForeignKey("DeletedBy")]
    public virtual User? DeletedByUser { get; set; }

    [ForeignKey("ItemId")]
    public virtual Item Items { get; set; } = null!;

    [ForeignKey("OrderId")]
    public virtual Order? Orders { get; set; } = null!;
}
