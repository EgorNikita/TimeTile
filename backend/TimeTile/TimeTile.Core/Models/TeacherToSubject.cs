using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models
{
    public class TeacherToSubject : AuditableEntity, IEntity
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }

        public int SubjectId { get; set; }

        public virtual InstitutionMember Teacher { get; set; } = null!;

        public virtual Subject Subject { get; set; } = null!;
    }
}
