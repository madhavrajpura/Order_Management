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
    public string Username { get; set; } = null!;

    [Required]
    [Column("Phone")]
    public int Phone { get; set; }

    [Column("Image")]
    public string? Image { get; set; }

    [Required]
    [Column("Password")]
    public string Password { get; set; } = null!;

    [Required]
    [Column("Email")]
    public string Email { get; set; } = null!;

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
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Column("DeletedAt")]
    public DateTime? DeletedAt { get; set; }

    [Column("CreatedBy")]
    public int? CreatedBy { get; set; }

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

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

}