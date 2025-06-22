using System.ComponentModel.DataAnnotations.Schema;
using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models;

public partial class Permission : AuditableEntity, IEntity
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    // Roles
    public virtual ICollection<RoleToPermission> RolesToPermission { get; set; } = new List<RoleToPermission>();

    [NotMapped]
    public virtual IEnumerable<Role> Roles => RolesToPermission.Select(x => x.Role);
}
