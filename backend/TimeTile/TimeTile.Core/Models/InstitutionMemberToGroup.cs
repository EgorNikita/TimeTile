using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTile.Core.Models
{
    public class InstitutionMemberToGroup : AuditableEntity
    {
        public int InstitutionMemberId { get; set; }

        public int GroupId { get; set; }

        public virtual InstitutionMember InstitutionMember { get; set; } = null!;

        public virtual Group Group { get; set; } = null!;
    }
}
