namespace TimeTile.Core.Models;

public partial class LessonToStudent
{
    public int Id { get; set; }

    public int LessonId { get; set; }

    public int StudentId { get; set; }

    public DateTimeOffset? CameAt { get; set; }

    public DateTimeOffset? LeftAt { get; set; }

    public int? ClassworkGradeId { get; set; }

    public int? HomeworkGradeId { get; set; }
    
    public virtual Grade? ClassworkGrade { get; set; }

    public virtual Grade? HomeworkGrade { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
