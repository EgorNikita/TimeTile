using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
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

    public DbSet<Classroom> Classrooms { get; set; } = null!;

    public DbSet<ClassroomType> ClassroomTypes { get; set; } = null!;

    public DbSet<Course> Courses { get; set; } = null!; 

    public DbSet<CourseToStudent> CoursesStudents { get; set; } = null!;

    public DbSet<CourseToUser> CoursesUsers { get; set; } = null!;

    public DbSet<File> Files { get; set; } = null!;

    public DbSet<Grade> Grades { get; set; } = null!; 

    public DbSet<Group> Groups { get; set; } = null!; 

    public DbSet<Institution> Institutions { get; set; } = null!; 

    public DbSet<Lesson> Lessons { get; set; } = null!; 

    public DbSet<LessonStatus> LessonStatuses { get; set; } = null!; 

    public DbSet<LessonToStudent> LessonsStudents { get; set; } = null!; 

    public DbSet<Permission> Permissions { get; set; } = null!; 
    
    public DbSet<Role> Roles { get; set; } = null!;

    public DbSet<RoleToPermission> RolesPermissions { get; set; } = null!;

    public DbSet<Student> Students { get; set; } = null!; 

    public DbSet<Subject> Subjects { get; set; } = null!; 

    public DbSet<InstitutionMember> InstitutionMembers { get; set; } = null!;

    public DbSet<InstitutionMemberToGroup> InstitutionMembersGroups { get; set; } = null!;

    public DbSet<TeacherToSubject> TeachersSubjects { get; set; } = null!;

    public DbSet<Term> Terms { get; set; } = null!; 
    
    public DbSet<TimetableUnit> TimetableUnits { get; set; } = null!; 

    public DbSet<User> Users { get; set; } = null!;
    
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = GetType().Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);

        AddConfigurationOnAuditableEntity(modelBuilder);

        OnModelCreatingPartial(modelBuilder);
    }

    private void AddConfigurationOnAuditableEntity(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.ClrType.IsAssignableTo(typeof(AuditableEntity)) &&
                entityType.BaseType == null)
            {
                var method = typeof(TimetileDbContext)
                    .GetMethod(nameof(ConfigureAuditableEntity), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(null, [ modelBuilder ]);
            }
        }
    }

    private static void ConfigureAuditableEntity<T>(ModelBuilder modelBuilder) where T : AuditableEntity
    {
        ConfigureFieldsForAuditableEntity<T>(modelBuilder);
        SetQueryFilterOnSoftDelete<T>(modelBuilder);
    }

    private static void ConfigureFieldsForAuditableEntity<T>(ModelBuilder modelBuilder) where T : AuditableEntity
    {
        modelBuilder.Entity<T>()
            .Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired(true);

        modelBuilder.Entity<T>()
            .Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired(true);

        modelBuilder.Entity<T>()
            .Property(e => e.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
    }

    private static void SetQueryFilterOnSoftDelete<T>(ModelBuilder modelBuilder) where T : AuditableEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => e.DeletedAt == null);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
