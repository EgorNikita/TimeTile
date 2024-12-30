namespace TimeTile.Core.Models;

public partial class Classroom : AuditableEntity
{
    public string Title { get; set; } = null!;

    public int InstitutionId { get; set; }

    public int AuditableEntityId { get; set; }

    public virtual Institution Institution { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
