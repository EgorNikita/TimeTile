using System.ComponentModel.DataAnnotations.Schema;
using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models;

public partial class Lesson : AuditableEntity, IEntity
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int ClassroomId { get; set; }

    public int LessonStatusId { get; set; }

    public DateTimeOffset Date { get; set; }

    public string Description { get; set; } = null!;

    public int? AssignmentId { get; set; }

    [NotMapped]
    public DateTimeOffset StartTime => LessonToTimetableUnits.Select(lt => lt.TimetableUnit).Min(t => t.StartTime);

    [NotMapped]
    public DateTimeOffset EndTime => LessonToTimetableUnits.Select(lt => lt.TimetableUnit).Max(t => t.EndTime);

    public virtual Classroom Classroom { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;

    public virtual LessonStatus LessonStatus { get; set; } = null!;

    public virtual ICollection<LessonToStudent> LessonsToStudents { get; set; } = new List<LessonToStudent>();

    [NotMapped]
    public virtual IEnumerable<Student> Students => LessonsToStudents.Select(x => x.Student);

    public virtual ICollection<LessonToTimetableUnit> LessonToTimetableUnits { get; set; } = new List<LessonToTimetableUnit>();

    public virtual Assignment? Assignment { get; set; }
}
