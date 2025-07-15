using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models
{
    public class Message : AuditableEntity, IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public string? Content { get; set; }
        public DateTimeOffset SentAt { get; set; }
        public DateTimeOffset? EditedAt { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;
    }
}
