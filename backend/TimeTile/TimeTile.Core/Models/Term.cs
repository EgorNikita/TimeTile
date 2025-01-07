namespace TimeTile.Core.Models;

public partial class Term : AuditableEntity
{
    public int InstitutionId { get; set; }
    
    public string Title { get; set; } = null!;
    
    public DateTimeOffset StartDate { get; set; }
    
    public DateTimeOffset EndDate { get; set; }
    
    public virtual Institution Institution { get; set; } = null!;
    
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}