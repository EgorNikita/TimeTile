namespace TimeTile.Core.Models;

public partial class TimetableUnit : AuditableEntity
{
    public string Title { get; set; } = null!;

    public int InstitutionId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }
    
    public virtual Institution Institution { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
