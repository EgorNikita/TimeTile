using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models
{
    public class SubmissionToFile : AuditableEntity, IEntity
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public int FileId { get; set; }
        public virtual Submission Submission { get; set; } = null!;
        public virtual File File { get; set; } = null!;
    }
}
