namespace TimeTile.Core.Models;

public partial class Teacher : User
{
    public virtual ICollection<ClassTeacher> ClassTeachers { get; set; } = new List<ClassTeacher>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
