using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models
{
    public class RoleToPermission : AuditableEntity, IEntity
    {
        public int Id { get; set; }

        public int RoleId { get; set; }

        public int PermissionId { get; set; }

        public virtual Role Role { get; set; } = null!;

        public virtual Permission Permission { get; set; } = null!;
    }
}
