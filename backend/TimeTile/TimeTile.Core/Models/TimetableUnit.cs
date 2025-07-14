using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models;

public partial class TimetableUnit : AuditableEntity, IInstitutionEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int InstitutionId { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }
    
    public virtual Institution Institution { get; set; } = null!;

    public virtual ICollection<LessonToTimetableUnit> LessonsToTimetableUnit { get; set; } = new List<LessonToTimetableUnit>();
}
