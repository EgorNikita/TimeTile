using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTile.Core.Models;

public partial class Subject : AuditableEntity
{
    public string Title { get; set; } = null!;
    
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    
    public virtual ICollection<Institution> Institutions { get; set; } = new List<Institution>();

    // Teachers
    public virtual ICollection<TeacherToSubject> TeachersToSubject { get; set; } = new List<TeacherToSubject>();

    [NotMapped]
    public virtual IEnumerable<InstitutionMember> Teachers => TeachersToSubject.Select(x => x.Teacher);
}
