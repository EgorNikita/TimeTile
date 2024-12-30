namespace TimeTile.Core.Models;

public partial class Subject : AuditableEntity
{
    public string Title { get; set; } = null!;
    
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    
    public virtual ICollection<Institution> Institutions { get; set; } = new List<Institution>();
}
