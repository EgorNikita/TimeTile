namespace TimeTile.Core.Models;

public partial class Institution : AuditableEntity
{
    public string Title { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    public virtual ICollection<LessonStatus> LessonStatuses { get; set; } = new List<LessonStatus>();
    
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    
    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    
    public virtual ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();

    public virtual ICollection<TimetableUnit> TimetableUnits { get; set; } = new List<TimetableUnit>();
    
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
    
    public virtual ICollection<Term> Terms { get; set; } = new List<Term>();
}
