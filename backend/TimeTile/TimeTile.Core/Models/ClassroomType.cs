using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTile.Core.Models
{
    public class ClassroomType : AuditableEntity
    {
        public string Description { get; set; } = null!;

        public int InstitutionId { get; set; }

        public int? IconId { get; set; }

        public virtual Institution Institution { get; set; } = null!;

        public virtual File? Icon { get; set; }

        public virtual ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();
    }
}
