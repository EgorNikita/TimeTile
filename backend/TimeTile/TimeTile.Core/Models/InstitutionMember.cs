using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTile.Core.Models;

public partial class InstitutionMember : User
{
    public int? PreferredClassroomId { get; set; }

    public int WeekWorkHours { get; set; }

    public virtual Classroom? Classroom { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    // Groups
    public virtual ICollection<InstitutionMemberToGroup> InstitutionMemberToGroups { get; set; } = new List<InstitutionMemberToGroup>();

    [NotMapped]
    public virtual IEnumerable<Group> ManagedGroups => InstitutionMemberToGroups.Select(x => x.Group);

    // Subjects
    public virtual ICollection<TeacherToSubject> TeacherToSubjects { get; set; } = new List<TeacherToSubject>();

    [NotMapped]
    public virtual IEnumerable<Subject> Subjects => TeacherToSubjects.Select(x => x.Subject);
}
