using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTile.Core.Models;

public partial class Subject : AuditableEntity, IInstitutionEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int InstitutionId { get; set; }
    
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    
    public virtual Institution Institution { get; set; } = null!;

    // Teachers
    public virtual ICollection<TeacherToSubject> TeachersToSubject { get; set; } = new List<TeacherToSubject>();

    [NotMapped]
    public virtual IEnumerable<InstitutionMember> Teachers => TeachersToSubject.Select(x => x.Teacher);
}
