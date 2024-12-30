namespace TimeTile.Core.Models;

public partial class Teacher : User
{
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
