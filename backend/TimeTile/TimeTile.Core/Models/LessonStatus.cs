using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models;

public partial class LessonStatus : AuditableEntity, IEntity
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
