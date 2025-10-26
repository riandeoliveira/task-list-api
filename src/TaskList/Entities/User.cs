using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskList.Entities;

[Table("users")]
public class User : BaseEntity
{
    [Column("name")]
    [MaxLength(64)]
    [MinLength(2)]
    [Required]
    public required string Name { get; set; }

    [Column("username")]
    [MaxLength(32)]
    [MinLength(4)]
    [RegularExpression(@"^[a-zA-Z0-9_.-]+$")]
    [Required]
    public required string Username { get; set; }

    [Column("email")]
    [EmailAddress]
    [MaxLength(64)]
    [MinLength(8)]
    [Required]
    public required string Email { get; set; }

    [Column("password")]
    [Required]
    public required string Password { get; set; }

    public ICollection<Task> Tasks { get; } = [];
}
