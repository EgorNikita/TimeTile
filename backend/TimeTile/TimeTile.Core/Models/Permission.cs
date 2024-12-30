namespace TimeTile.Core.Models;

public partial class Permission : AuditableEntity
{
    public string Description { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
