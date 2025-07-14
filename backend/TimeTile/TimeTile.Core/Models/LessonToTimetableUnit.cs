using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models
{
    public class LessonToTimetableUnit : AuditableEntity, IEntity
    {
        public int Id { get; set; }

        public int LessonId { get; set; }

        public int TimetableUnitId { get; set; }

        public virtual Lesson Lesson { get; set; } = null!;

        public virtual TimetableUnit TimetableUnit { get; set; } = null!;
    }
}
