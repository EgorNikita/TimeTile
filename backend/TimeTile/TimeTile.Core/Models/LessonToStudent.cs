using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models;

public partial class LessonToStudent : AuditableEntity, IEntity
{
    public int Id { get; set; }

    public int LessonId { get; set; }

    public int StudentId { get; set; }

    public DateTimeOffset? CameAt { get; set; }

    public DateTimeOffset? LeftAt { get; set; }

    public int? GradeId { get; set; }
    
    public virtual Grade? Grade { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
