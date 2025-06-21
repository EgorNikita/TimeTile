using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models;

public partial class Classroom : AuditableEntity, IInstitutionEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int Capacity { get; set; }

    public int InstitutionId { get; set; }
    
    public int ClassroomTypeId { get; set; }

    public virtual Institution Institution { get; set; } = null!;

    public virtual ClassroomType ClassroomType { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual ICollection<InstitutionMember> InstitutionMembers { get; set; } = new List<InstitutionMember>();
}
