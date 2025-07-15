using System.ComponentModel.DataAnnotations.Schema;
using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models;

public partial class Course : AuditableEntity, IInstitutionEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;
    
    public int SubjectId { get; set; }

    public int TeacherId { get; set; }

    public bool IsAdvanced { get; set; } = false;
    
    public int InstitutionId { get; set; }
    
    public int TermId { get; set; }

    public int IconId { get; set; }

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual Institution Institution { get; set; } = null!;
    
    public virtual Term Term { get; set; } = null!;
    
    public virtual Subject Subject { get; set; } = null!;

    public virtual InstitutionMember Teacher { get; set; } = null!;

    public virtual File Icon { get; set; } = null!;

    // CoursesToStudents
    public virtual ICollection<CourseToStudent> CoursesToStudents { get; set; } = new List<CourseToStudent>();

    [NotMapped]
    public virtual IEnumerable<Student> Students => CoursesToStudents.Select(x => x.Student);

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
