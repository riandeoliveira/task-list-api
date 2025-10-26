using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskList.Entities;

[Table("password_reset_tokens")]
public class PasswordResetToken : BaseEntity
{
    [Column("value")]
    [Required]
    public required string Value { get; set; }

    [Column("expires_at")]
    [Required]
    public required DateTime ExpiresAt { get; set; }

    [Column("user_id")]
    [ForeignKey("User")]
    [Required]
    public required Guid UserId { get; set; }
}
