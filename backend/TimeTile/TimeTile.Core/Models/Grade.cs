using TimeTile.Core.Enums;
using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models;

public partial class Grade : AuditableEntity, IEntity
{
    public int Id { get; set; }

    public float Value { get; set; }
    
    public float Weight { get; set; }

    public GradeType Type { get; set; }

    public virtual LessonToStudent? LessonToStudent { get; set; }
    
    public virtual CourseToStudent? CourseToStudent { get; set; }

    public virtual Submission? Submission { get; set; }
}
