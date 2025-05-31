namespace TimeTile.Core.Models;

public partial class Grade : AuditableEntity
{
    public int Id { get; set; }

    public short Value { get; set; }
    
    public float Weight { get; set; }

    public virtual LessonToStudent? LessonToStudentClasswork { get; set; }

    public virtual LessonToStudent? LessonToStudentHomework { get; set; }
    
    public virtual CourseToStudent? CourseToStudent { get; set; }
}
