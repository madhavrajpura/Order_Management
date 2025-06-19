using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;

[Table("Role", Schema = "public")]
public class Role
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [Column("Name")]
    [StringLength(30)]
    public string Name { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();

}
