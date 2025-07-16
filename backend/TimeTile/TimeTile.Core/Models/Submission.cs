using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Enums;
using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models
{
    public class Submission : AuditableEntity, IEntity
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public int? GradeId { get; set; }
        public SubmissionStatus Status { get; set; } = SubmissionStatus.NotSubmitted;
        public string? StudentNote { get; set; }
        public string? Feedback { get; set; }
        public DateTimeOffset? SubmittedAt { get; set; }

        public virtual Assignment Assignment { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
        public virtual Grade? Grade { get; set; }

        //SubmissionToFiles
        public virtual ICollection<SubmissionToFile> SubmissionToFiles { get; set; } = new List<SubmissionToFile>();

        [NotMapped]
        public virtual IEnumerable<File> Files => SubmissionToFiles.Select(x => x.File);
    }
}
