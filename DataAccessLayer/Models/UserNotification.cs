using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;

[Table("UserNotification", Schema = "public")]
public class UserNotification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [ForeignKey("User")]
    [Column("UserId")]
    public int UserId { get; set; }

    [Required]
    [ForeignKey("Notification")]
    [Column("NotificationId")]
    public int NotificationId { get; set; }

    [Required]
    [Column("IsRead")]
    [DefaultValue("false")]
    public bool IsRead { get; set; } = false;

    [Column("ReadAt", TypeName = "timestamp without time zone")]
    public DateTime? ReadAt { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual Notification Notification { get; set; } = null!;

}