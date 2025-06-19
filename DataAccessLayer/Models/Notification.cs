using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;


[Table("Notification", Schema = "public")]
public class Notification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [Column("Message")]
    public string Message { get; set; }

    [Required]
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required]
    [ForeignKey("User")]
    [Column("CreatedBy")]
    public int CreatedBy { get; set; }

    [Required]
    [Column("IsActive")]
    public bool IsActive { get; set; } = true;

    public virtual User User { get; set; } = null!;

}