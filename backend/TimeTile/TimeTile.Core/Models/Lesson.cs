namespace TimeTile.Core.Models;

public partial class Lesson : AuditableEntity
{
    public int TimetableUnitId { get; set; }

    public int CourseId { get; set; }

    public int ClassroomId { get; set; }

    public int LessonStatusId { get; set; }

    public DateOnly Date { get; set; }

    public string Description { get; set; } = null!;

    public string HomeworkDescription { get; set; } = null!;
    
    public virtual Classroom Classroom { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;

    public virtual LessonStatus LessonStatus { get; set; } = null!;

    public virtual ICollection<LessonToStudent> LessonsToStudents { get; set; } = new List<LessonToStudent>();

    public virtual TimetableUnit TimetableUnit { get; set; } = null!;
}
