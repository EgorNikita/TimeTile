namespace TimeTile.Core.Models;

public partial class LessonStatus : AuditableEntity
{
    public string Description { get; set; } = null!;
    
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    
    public virtual ICollection<Institution> Institutions { get; set; } = new List<Institution>();
}
