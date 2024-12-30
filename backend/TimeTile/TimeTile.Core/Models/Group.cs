namespace TimeTile.Core.Models;

public partial class Group : AuditableEntity
{
    public string Title { get; set; } = null!;

    public int InstitutionId { get; set; }
    
    public virtual ICollection<ClassTeacher> ClassTeachers { get; set; } = new List<ClassTeacher>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    
    public virtual Institution Institution { get; set; } = null!;
}
