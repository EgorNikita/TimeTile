namespace TimeTile.Core.Models;

public partial class CourseToStudent : AuditableEntity
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int StudentId { get; set; }

    public int? ExamGradeId { get; set; }
    
    public bool HasExam { get; set; }

    public int PositionX { get; set; }

    public int PositionY { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
    
    public virtual Grade? ExamGrade { get; set; }
}
