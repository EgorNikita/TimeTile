using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTile.Core.Models
{
    public class TeacherToSubject : AuditableEntity
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }

        public int SubjectId { get; set; }

        public virtual InstitutionMember Teacher { get; set; } = null!;

        public virtual Subject Subject { get; set; } = null!;
    }
}
