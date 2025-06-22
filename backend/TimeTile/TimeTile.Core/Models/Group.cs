using System.ComponentModel.DataAnnotations.Schema;
using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models;

public partial class Group : AuditableEntity, IInstitutionEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int InstitutionId { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    
    public virtual Institution Institution { get; set; } = null!;

    public virtual ICollection<InstitutionMemberToGroup> InstitutionMembersToGroup { get; set; } = new List<InstitutionMemberToGroup>();

    [NotMapped]
    public virtual IEnumerable<InstitutionMember> GroupMentors => InstitutionMembersToGroup.Select(x => x.InstitutionMember);
}
