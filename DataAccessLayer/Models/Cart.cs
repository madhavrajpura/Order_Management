using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;

[Table("Cart", Schema = "public")]
public class Cart
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
    [ForeignKey("User")]
    [Column("UserId")]
    public int UserId { get; set; }

    [Required]
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual User User { get; set; } = null!;
    public virtual Item Item { get; set; } = null!;
}