namespace TimeTile.Core.Models;

public partial class Course : AuditableEntity
{
    public string Title { get; set; } = null!;
    
    public int SubjectId { get; set; }

    public int TeacherId { get; set; }

    public bool IsAdvanced { get; set; } = false;
    
    public int InstitutionId { get; set; }
    
    public int TermId { get; set; }

    public virtual ICollection<CourseToStudent> CoursesToStudents { get; set; } = new List<CourseToStudent>();

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual Institution Institution { get; set; } = null!;
    
    public virtual Term Term { get; set; } = null!;
    
    public virtual Subject Subject { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
