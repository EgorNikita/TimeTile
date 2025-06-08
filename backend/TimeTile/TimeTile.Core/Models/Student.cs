using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTile.Core.Models;

public partial class Student : User
{
    public int? GroupId { get; set; }

    public virtual Group? Group { get; set; }

    // LessonsToStudents
    public virtual ICollection<LessonToStudent> LessonsToStudents { get; set; } = new List<LessonToStudent>();

    [NotMapped]
    public virtual IEnumerable<Lesson> Lessons => LessonsToStudents.Select(x => x.Lesson);

    // CoursesToStudents
    public virtual ICollection<CourseToStudent> CoursesToStudents { get; set; } = new List<CourseToStudent>();

    [NotMapped]
    public virtual IEnumerable<Course> Courses => CoursesToStudents.Select(x => x.Course);
}
