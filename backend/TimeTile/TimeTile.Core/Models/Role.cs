using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTile.Core.Models;

public class Role : AuditableEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;
    
    public int InstitutionId { get; set; }
    
    public virtual Institution Institution { get; set; } = null!;
    
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    // Permissions
    public virtual ICollection<RoleToPermission> RoleToPermissions { get; set; } = new List<RoleToPermission>();

    [NotMapped]
    public virtual IEnumerable<Permission> Permissions => RoleToPermissions.Select(x => x.Permission);
}