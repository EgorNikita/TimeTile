namespace TimeTile.Core.Models;

public partial class Student : User
{
    public int GroupId { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual ICollection<LessonToStudent> LessonsToStudents { get; set; } = new List<LessonToStudent>();
    
    public virtual ICollection<CourseToStudent> CoursesToStudents { get; set; } = new List<CourseToStudent>();
}
