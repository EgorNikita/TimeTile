namespace TimeTile.Core.Models;

public partial class LessonStatus : AuditableEntity
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public int ArgbColor { get; set; }

    public int InstitutionId { get; set; }

    public virtual Institution Institution { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
