namespace TimeTile.Core.Models;

public partial class ClassTeacher : Teacher
{
    public int GroupId { get; set; }

    public virtual Group Group { get; set; } = null!;
}
