using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;

[Table("WishList", Schema = "public")]
public class WishList
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
    [Column("LikedAt", TypeName = "timestamp without time zone")]
    [DefaultValue("now()")]
    public DateTime LikedAt { get; set; } = DateTime.Now;

    [Required]
    [ForeignKey("User")]
    [Column("LikedBy")]
    public int LikedBy { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Item Item { get; set; } = null!;
}