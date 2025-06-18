using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;

[Table("User", Schema = "public")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [Column("Username")]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [Column("Password")]
    public string Password { get; set; }

    [Required]
    [Column("Email")]
    public string Email { get; set; }

    [Required]
    [ForeignKey("Role")]
    [Column("RoleId")]
    public int RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;

    [Column("IsDelete")]
    public bool IsDelete { get; set; } = false;

    [Column("LogoutAt")]
    public DateTime? LogoutAt { get; set; }

    [Required]
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Column("DeletedAt")]
    public DateTime? DeletedAt { get; set; }

    [Required]
    // [ForeignKey("User")]
    [Column("CreatedBy")]
    public int CreatedBy { get; set; }

    // [ForeignKey("User")]
    [Column("UpdatedBy")]
    public int? UpdatedBy { get; set; }

    // [ForeignKey("User")]
    [Column("DeletedBy")]
    public int? DeletedBy { get; set; }
    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; }

    [ForeignKey("UpdatedBy")]
    public virtual User? UpdatedByUser { get; set; }

    [ForeignKey("DeletedBy")]
    public virtual User? DeletedByUser { get; set; }

    public ICollection<Item> Items { get; set; } = new List<Item>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();

}