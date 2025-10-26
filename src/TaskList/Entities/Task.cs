using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskList.Entities;

[Table("tasks")]
public class Task : BaseEntity
{
    [Column("title")]
    [MaxLength(128)]
    [MinLength(4)]
    [Required]
    public required string Title { get; set; }

    [Column("description")]
    [MaxLength(1024)]
    public string Description { get; set; } = "";

    [Column("is_completed")]
    [Required]
    public bool IsCompleted { get; set; } = false;

    [Column("user_id")]
    [ForeignKey("User")]
    [Required]
    public required Guid UserId { get; set; }
}
