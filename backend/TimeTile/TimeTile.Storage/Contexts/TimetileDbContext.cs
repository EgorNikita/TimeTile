using Microsoft.EntityFrameworkCore;
using TimeTile.Core.Models;
using TimeTile.Storage.Utils;
using File = TimeTile.Core.Models.File;

namespace TimeTile.Storage.Contexts;

public sealed partial class TimetileDbContext : DbContext
{
    public TimetileDbContext()
    {
    }

    public TimetileDbContext(DbContextOptions<TimetileDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .AddInterceptors(new AuditingSaveChangesInterceptor())
            .UseLazyLoadingProxies()
            .EnableSensitiveDataLogging();

    public DbSet<AuditableEntity> AuditableEntities { get; set; } = null!; 

    public DbSet<Classroom> Classrooms { get; set; } = null!; 

    public DbSet<Course> Courses { get; set; } = null!; 

    public DbSet<CourseToStudent> CoursesStudents { get; set; } = null!; 

    public DbSet<File> Files { get; set; } = null!;

    public DbSet<Grade> Grades { get; set; } = null!; 

    public DbSet<Group> Groups { get; set; } = null!; 

    public DbSet<Institution> Institutions { get; set; } = null!; 

    public DbSet<Lesson> Lessons { get; set; } = null!; 

    public DbSet<LessonStatus> LessonStatuses { get; set; } = null!; 

    public DbSet<LessonToStudent> LessonsStudents { get; set; } = null!; 

    public DbSet<Permission> Permissions { get; set; } = null!; 
    
    public DbSet<Role> Roles { get; set; } = null!; 

    public DbSet<Student> Students { get; set; } = null!; 

    public DbSet<Subject> Subjects { get; set; } = null!; 

    public DbSet<InstitutionMember> InstitutionMembers { get; set; } = null!;

    public DbSet<InstitutionMemberToGroup> InstitutionMembersGroups { get; set; } = null!;

    public DbSet<TeacherToSubject> TeachersSubjects { get; set; } = null!;

    public DbSet<Term> Terms { get; set; } = null!; 
    
    public DbSet<TimetableUnit> TimetableUnits { get; set; } = null!; 

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = GetType().Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
