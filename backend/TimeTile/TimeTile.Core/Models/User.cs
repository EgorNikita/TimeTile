namespace TimeTile.Core.Models;

public partial class User : AuditableEntity
{
    public string AvatarPath { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string HomeAddress { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
    
    public DateOnly BirthDate { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }
    
    public int InstitutionId { get; set; }
    
    public virtual Institution Institution { get; set; } = null!;
    
    public virtual Role Role { get; set; } = null!;
}
