namespace TimeTile.Core.Models;

public partial class User : AuditableEntity
{
    public int Id { get; set; }

    public int AvatarId { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string HomeAddress { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
    
    public DateOnly BirthDate { get; set; }

    public string Login { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }
    
    public int? InstitutionId { get; set; }

    public virtual File Avatar { get; set; } = null!;

    public virtual Institution? Institution { get; set; }
    
    public virtual Role Role { get; set; } = null!;
}
