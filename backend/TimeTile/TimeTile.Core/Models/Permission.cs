using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTile.Core.Models;

public partial class Permission : AuditableEntity
{
    public string Description { get; set; } = null!;

    // Roles
    public virtual ICollection<RoleToPermission> RolesToPermission { get; set; } = new List<RoleToPermission>();

    [NotMapped]
    public virtual IEnumerable<Role> Roles => RolesToPermission.Select(x => x.Role);
}
