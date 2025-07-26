using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models;

public partial class CourseToStudent : AuditableEntity, IEntity
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int StudentId { get; set; }

    public int? GradeId { get; set; }

    public short PositionX { get; set; }

    public short PositionY { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
    
    public virtual Grade? Grade { get; set; }
}
