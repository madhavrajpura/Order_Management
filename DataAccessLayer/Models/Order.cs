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
    [Column("TotalAmount", TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [Column("OrderDate")]
    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Column("DeliveryDate")]
    public DateTime? DeliveryDate { get; set; }

    [Column("IsDelete")]
    public bool IsDelete { get; set; } = false;
    
    [Column("IsDelivered")]
    public bool IsDelivered { get; set; } = false;

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
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    public virtual User? UpdatedByUser { get; set; }

    [ForeignKey("DeletedBy")]
    public virtual User? DeletedByUser { get; set; }
    public virtual ICollection<OrderItem>? OrderItems { get; set; }

}