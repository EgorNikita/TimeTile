using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models
{
    public class CourseToUser : AuditableEntity, IEntity
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public int UserId { get; set; }

        public int OrderNumber { get; set; }

        public virtual Course Course { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
