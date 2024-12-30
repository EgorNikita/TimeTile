namespace TimeTile.Core.Models;

public class Role : AuditableEntity
{
    public string Title { get; set; } = null!;
    
    public int InstitutionId { get; set; }
    
    public virtual Institution Institution { get; set; } = null!;
    
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}