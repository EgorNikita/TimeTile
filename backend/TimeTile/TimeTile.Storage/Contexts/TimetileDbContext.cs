using Microsoft.EntityFrameworkCore;
using TimeTile.Core.Models;

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
            .UseLazyLoadingProxies()
            .EnableSensitiveDataLogging();

    public DbSet<AuditableEntity> AuditableEntities { get; set; } = null!; 

    public DbSet<ClassTeacher> ClassTeachers { get; set; } = null!; 

    public DbSet<Classroom> Classrooms { get; set; } = null!; 

    public DbSet<Course> Courses { get; set; } = null!; 

    public DbSet<CourseToStudent> CoursesStudents { get; set; } = null!; 

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

    public DbSet<Teacher> Teachers { get; set; } = null!; 

    public DbSet<Term> Terms { get; set; } = null!; 
    
    public DbSet<TimetableUnit> TimetableUnits { get; set; } = null!; 

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditableEntity>(entity =>
        {
            // Table configuration
            entity.ToTable("auditable_entities");
            entity.HasKey(e => e.Id);

            // Use TPT inheritance mapping strategy
            entity.UseTptMappingStrategy();

            // Property configurations
            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at")
                .IsRequired();

            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at")
                .IsRequired(false); // Nullable for soft deletion
        });

        modelBuilder.Entity<ClassTeacher>(entity =>
        {
            // Table configuration
            entity.ToTable("class_teachers");

            // Property configurations
            entity.Property(e => e.GroupId)
                .HasColumnName("group_id");
            
            // Relationships
            entity.HasOne(d => d.Group)
                .WithMany(p => p.ClassTeachers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("class_teachers_group_id_fkey");
        });

        modelBuilder.Entity<Classroom>(entity =>
        {
            // Table configuration
            entity.ToTable("classrooms", t =>
                t.HasCheckConstraint(
                    "CHK_Classroom_Title_Valid",
                    "\"title\" ~ '^[a-zA-Z \\d-]+$'"
                ));

            // Unique index for InstitutionId and Title
            entity.HasIndex(e => new { e.InstitutionId, e.Title })
                .IsUnique()
                .HasDatabaseName("classrooms_institution_title_key");
            
            // Property configurations
            entity.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");
            
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Institution)
                .WithMany(p => p.Classrooms)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("classrooms_institution_id_fkey");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            // Table configuration
            entity.ToTable("courses", t =>
                t.HasCheckConstraint(
                    "CHK_Course_Title_Valid",
                    "\"title\"  ~ '^[\\w -.*+,]+$'"
                ));
            
            entity.HasIndex(e => new {e.Title, e.SubjectId, e.TeacherId, e.InstitutionId, e.TermId })
                .IsUnique()
                .HasDatabaseName("courses_title_subject_teacher_institution_term_key");
            
            // Property configurations
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            
            entity.Property(e => e.IsAdvanced)
                .HasDefaultValue(false)
                .HasColumnName("is_advanced");

            entity.Property(e => e.SubjectId)
                .HasColumnName("subject_id");

            entity.Property(e => e.TeacherId)
                .HasColumnName("teacher_id");

            entity.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");
            
            entity.Property(e => e.TermId)
                .HasColumnName("term_id");
            
            // Relationships
            entity.HasOne(d => d.Subject)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_subject_id_fkey");

            entity.HasOne(d => d.Teacher)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_teacher_id_fkey");

            entity.HasOne(d => d.Institution)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_institution_id_fkey");

            entity.HasOne(d => d.Term)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.TermId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_term_id_fkey");
        });

        modelBuilder.Entity<CourseToStudent>(entity =>
        {
            // Table and Key Configuration
            entity.ToTable("courses_students", t =>
                t.HasCheckConstraint(
                    "CK_CoursesStudents_HasExam_ExamGrade",
                    "\"has_exam\" = FALSE OR \"exam_grade_id\" IS NOT NULL"
                ));
            
            entity.HasKey(e => e.Id).HasName("courses_students_pkey");
            
            entity.HasIndex(e => new { e.CourseId, e.StudentId }, "courses_students_course_id_student_id_key")
                .IsUnique();
            
            // Property Configurations
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.ExamGradeId)
                .HasColumnName("exam_grade_id")
                .IsRequired(false);
            entity.Property(e => e.HasExam)
                .HasColumnName("has_exam");

            // Relationships
            entity.HasOne(d => d.Course)
                .WithMany(p => p.CoursesToStudents)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_students_course_id_fkey");

            entity.HasOne(d => d.Student)
                .WithMany(p => p.CoursesToStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_students_student_id_fkey");

            entity.HasOne(d => d.ExamGrade)
                .WithOne(p => p.CourseToStudent)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_students_exam_grade_id_fkey");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            // Table and Key Configuration
            entity.ToTable("grades", t =>
            {
                t.HasCheckConstraint("CHK_Grade_Value_Positive", "\"value\" > 0");
                t.HasCheckConstraint("CHK_Grade_Weight_Positive", "\"weight\" > 0");
            });
            
            // Property Configurations
            entity.Property(e => e.Value)
                .HasColumnName("value");

            entity.Property(e => e.Weight)
                .HasColumnName("weight")
                .HasDefaultValue((float)1.0);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            // Table and Index Configuration
            entity.ToTable("groups", t =>
                t.HasCheckConstraint(
                    "CHK_Group_Title_Valid",
                    "\"title\"  ~ '^[\\w -.*]+$'"
                ));

            entity.HasIndex(e => new { e.InstitutionId, e.Title })
                .HasDatabaseName("groups_institution_title_key")
                .IsUnique();

            // Property Configurations
            entity.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");

            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            // Relationships
            entity.HasOne(d => d.Institution)
                .WithMany(p => p.Groups)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("groups_institution_id_fkey");
        });
        
        modelBuilder.Entity<Institution>(entity =>
        {
            // Table Configuration
            entity.ToTable("institutions", t =>
            {
                // Check constraint for Title to allow only letters, digits, spaces, and special characters
                t.HasCheckConstraint("CHK_Institution_Title_NotEmpty", 
                    "\"title\" ~ '^[\\w \\-.*&\"'',\\/\\\\|]+$'");

                // Check constraint for Address to allow only letters, digits, spaces, and special characters
                t.HasCheckConstraint("CHK_Institution_Address_NotEmpty", 
                    "\"address\" ~ '^[A-Za-z\\d''\\.\\- \\,]$'");

                // Check constraint for Email (valid format)
                t.HasCheckConstraint("CHK_Institution_Email_Valid", 
                    "\"email\" ~ '^[A-Za-z\\d._%+-]+@[A-Za-z\\d.-]+\\.[A-Za-z]{2,}$'");

                // Check constraint for PhoneNumber (digits and optional formatting characters)
                t.HasCheckConstraint("CHK_Institution_Phone_Valid",
                    "\"phone_number\" ~ '^(\\+\\d{1,2} )?\\(?\\d{3}\\)?[ .-]\\d{3}[ .-]\\d{4}$'");
            });

            entity.HasIndex(e => e.Title)
                .HasDatabaseName("institutions_title_key")
                .IsUnique();
            
            // Property Configurations
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");

            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            // Table Configuration
            entity.ToTable("lessons");

            entity.HasIndex(e => new { e.CourseId, e.TimetableUnitId, e.Date })
                .HasDatabaseName("lessons_course_timetable_date_key")
                .IsUnique();
            
            // Property Configurations
            entity.Property(e => e.ClassroomId)
                .HasColumnName("classroom_id");

            entity.Property(e => e.CourseId)
                .HasColumnName("course_id");

            entity.Property(e => e.Date)
                .HasColumnName("date");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");

            entity.Property(e => e.HomeworkDescription)
                .HasMaxLength(255)
                .HasColumnName("homework_description");

            entity.Property(e => e.LessonStatusId)
                .HasColumnName("lesson_status_id");

            entity.Property(e => e.TimetableUnitId)
                .HasColumnName("timetable_unit_id");

            // Relationship Configurations
            entity.HasOne(d => d.Classroom)
                .WithMany(p => p.Lessons)
                .HasForeignKey(d => d.ClassroomId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_classroom_id_fkey");

            entity.HasOne(d => d.Course)
                .WithMany(p => p.Lessons)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_course_id_fkey");

            entity.HasOne(d => d.LessonStatus)
                .WithMany(p => p.Lessons)
                .HasForeignKey(d => d.LessonStatusId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_lesson_status_id_fkey");

            entity.HasOne(d => d.TimetableUnit)
                .WithMany(p => p.Lessons)
                .HasForeignKey(d => d.TimetableUnitId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_timetable_unit_id_fkey");
        });

        modelBuilder.Entity<LessonStatus>(entity =>
        {
            // Table Configuration
            entity.ToTable("lesson_statuses", t => 
                t.HasCheckConstraint(
                    "CHK_LessonStatus_Description_Valid",
                    "\"description\"  ~ '^[a-zA-Z\\d ]+$'"
                ));

            entity.HasIndex(e => e.Description)
                .HasDatabaseName("lesson_statuses_description_key")
                .IsUnique();
            
            // Property Configuration
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            
            // Many-to-Many Configuration
            entity.HasMany(e => e.Institutions)
                .WithMany(e => e.LessonStatuses)
                .UsingEntity<Dictionary<string, object>>(
                    "LessonStatusInstitution",
                    j => j
                        .HasOne<Institution>()
                        .WithMany()
                        .HasForeignKey("institution_id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("lesson_status_institution_institution_id_fkey"),
                    j => j
                        .HasOne<LessonStatus>()
                        .WithMany()
                        .HasForeignKey("lesson_status_id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("lesson_status_institution_lesson_status_id_fkey"),
                    j =>
                    {
                        j.ToTable("lesson_statuses_institutions");
                        j.HasKey("lesson_status_id", "institution_id")
                            .HasName("lesson_status_institution_pkey");
                    });
        });

        modelBuilder.Entity<LessonToStudent>(entity =>
        {
            // Define the primary key
            entity.HasKey(e => e.Id).HasName("lessons_students_pkey");

            // Map to table and create index
            entity.ToTable("lessons_students", t =>
            {
                t.HasCheckConstraint(
                    "CHK_LessonToStudent_CameAt_LessThan_LeftAt",
                    "(\"came_at\" < \"left_at\") OR (\"left_at\" IS NULL)"
                );
                t.HasCheckConstraint(
                    "CHK_LessonToStudent_CameAt_IsNotNull_OR_CameAt_LeftAt_IsNull",
                    "(\"came_at\" IS NULL AND \"left_at\" IS NULL) OR (\"came_at\" IS NOT NULL)"
                );
            });
            
            entity.HasIndex(e => new { e.LessonId, e.StudentId }, "lessons_students_lesson_id_student_id_key")
                .IsUnique();

            // Define properties with column names
            entity.Property(e => e.Id)
                .HasColumnName("id");
            
            entity.Property(e => e.LessonId)
                .HasColumnName("lesson_id");
            
            entity.Property(e => e.StudentId)
                .HasColumnName("student_id");
            
            entity.Property(e => e.CameAt)
                .HasColumnName("came_at")
                .IsRequired(false);
            
            entity.Property(e => e.LeftAt)
                .HasColumnName("left_at")
                .IsRequired(false);
            
            entity.Property(e => e.ClassworkGradeId)
                .HasColumnName("classwork_grade_id")
                .IsRequired(false);
            
            entity.Property(e => e.HomeworkGradeId)
                .HasColumnName("homework_grade_id")
                .IsRequired(false);
            
            // Define relationships
            entity.HasOne(d => d.ClassworkGrade)
                .WithOne(p => p.LessonToStudentClasswork)
                .HasForeignKey<LessonToStudent>(d => d.ClassworkGradeId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_students_classwork_grade_id_fkey");

            entity.HasOne(d => d.HomeworkGrade)
                .WithOne(p => p.LessonToStudentHomework)
                .HasForeignKey<LessonToStudent>(d => d.HomeworkGradeId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_students_homework_grade_id_fkey");

            entity.HasOne(d => d.Lesson)
                .WithMany(p => p.LessonsToStudents)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_students_lesson_id_fkey");

            entity.HasOne(d => d.Student)
                .WithMany(p => p.LessonsToStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_students_student_id_fkey");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            // Table Configuration
            entity.ToTable("permissions", t => 
                t.HasCheckConstraint(
                    "CHK_Permission_Description_Valid",
                    "\"description\"  ~ '^[\\w -]+$'"
                ));
            
            entity.HasIndex(e => e.Description, "permissions_description_key").IsUnique();
            
            // Property Configuration
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            
            entity.HasMany(p => p.Roles)  // A Permission can have many Roles
                .WithMany(r => r.Permissions)  // A Role can have many Permissions
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermissions",  // Name of the join table
                    j => j
                        .HasOne<Role>()  // The join table has one Role
                        .WithMany()  // A Role can be related to many Permissions
                        .HasForeignKey("role_id")  // Foreign key for Role
                        .OnDelete(DeleteBehavior.NoAction)  // No action on delete
                        .HasConstraintName("role_permissions_role_id_fkey"),  // Foreign key constraint name
                    j => j
                        .HasOne<Permission>()  // The join table has one Permission
                        .WithMany()  // A Permission can be related to many Roles
                        .HasForeignKey("permission_id")  // Foreign key for Permission
                        .OnDelete(DeleteBehavior.NoAction)  // No action on delete
                        .HasConstraintName("role_permissions_permission_id_fkey"),  // Foreign key constraint name
                    j =>
                    {
                        j.ToTable("roles_permissions");  // Name of the join table
                        j.HasKey("role_id", "permission_id")  // Composite primary key
                            .HasName("role_permissions_pkey");  // Name of the composite primary key
                    });
        });

        modelBuilder.Entity<Role>(entity =>
        {
            // Table Configuration
            entity.ToTable("roles", t => 
                t.HasCheckConstraint(
                    "CHK_Role_Title_Valid",
                    "\"title\"  ~ '^[\\w -]+$'"
                ));
            
            entity.HasIndex(e => new {e.InstitutionId, e.Title}, "roles_title_institution_key").IsUnique();
            
            // Property Configuration
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            
            entity.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");
            
            // Relationships
            entity.HasOne(d => d.Institution)
                .WithMany(p => p.Roles)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("roles_institution_id_fkey");
        });
        
        modelBuilder.Entity<Student>(entity =>
        {
            // Table Configuration
            entity.ToTable("students");

            entity.HasIndex(e => new { e.Id, e.GroupId })
                .HasDatabaseName("students_id_group_key")
                .IsUnique();
            
            // Property Configuration
            entity.Property(e => e.GroupId)
                .HasColumnName("group_id");
            
            // Relationship Configuration
            entity.HasOne(d => d.Group).WithMany(p => p.Students)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("students_group_id_fkey");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            // Table Configuration
            entity.ToTable("subjects", t => 
                t.HasCheckConstraint(
                    "CHK_Subject_Title_Valid",
                    "\"title\" ~ '^[\\w -]+$'"
                ));
            
            entity.HasIndex(e => e.Title, "subjects_title_key").IsUnique();

            // Property Configuration
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            
            // Many-to-Many Configuration
            entity.HasMany(s => s.Institutions)
                .WithMany(i => i.Subjects)
                .UsingEntity<Dictionary<string, object>>(
                    "SubjectInstitution",
                    j => j
                        .HasOne<Institution>()
                        .WithMany()
                        .HasForeignKey("institution_id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("subjects_institutions_institution_id_fkey"),
                    j => j
                        .HasOne<Subject>()
                        .WithMany()
                        .HasForeignKey("subject_id") 
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("subjects_institutions_subject_id_fkey"),
                    j =>
                    {
                        j.ToTable("subjects_institutions");  // Name of the join table
                        j.HasKey("institution_id", "subject_id")  // Composite primary key
                            .HasName("subjects_institutions_pkey");  // Name of the composite primary key
                    });
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            // Table Configuration
            entity.ToTable("teachers");
            
            entity.UseTptMappingStrategy();
        });

        modelBuilder.Entity<Term>(entity =>
        {
            // Table Configuration
            entity.ToTable("terms", t =>
            {
                t.HasCheckConstraint(
                    "CHK_Term_Title_Valid",
                    "\"title\" ~ '^[\\w -.*+,]+$'"
                );
                t.HasCheckConstraint(
                    "CHK_Term_StartDate_LessThan_EndDate",
                    "\"start_date\" < \"end_date\""
                );
            });

            entity.HasIndex(e => new { e.InstitutionId, e.Title })
                .HasDatabaseName("terms_institution_title_key")
                .IsUnique();
            
            entity.HasIndex(e => new { e.StartDate, e.EndDate, e.InstitutionId })
                .HasDatabaseName("terms_institution_start_end_key")
                .IsUnique();
            
            // Property Configuration
            entity.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");

            entity.Property(e => e.Title)
                .HasColumnName("title");

            entity.Property(e => e.StartDate)
                .HasColumnName("start_date");
            
            entity.Property(e => e.EndDate)
                .HasColumnName("end_date");
            
            // Relationships
            entity.HasOne(d => d.Institution)
                .WithMany(p => p.Terms)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("terms_institution_id_fkey");
        });
        
        modelBuilder.Entity<TimetableUnit>(entity =>
        {
            // Table Configuration
            entity.ToTable("timetable_units", t =>
            {
                t.HasCheckConstraint(
                    "CHK_TimetableUnit_Title_Valid", 
                    "\"title\" ~ '^[\\w ]+$'"
                );
                t.HasCheckConstraint(
                    "CHK_TimetableUnit_StartTime_LessThan_EndTime",
                    "\"start_time\" < \"end_time\""
                );
            });

            entity.HasIndex(e => new { e.InstitutionId, e.Title })
                .HasDatabaseName("timetable_units_institution_title_key")
                .IsUnique();
            
            entity.HasIndex(e => new { e.InstitutionId, e.Title, e.StartTime, e.EndTime })
                .HasDatabaseName("timetable_units_institution_title_start_end_key")
                .IsUnique();
            
            // Property Configuration
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.InstitutionId).HasColumnName("institution_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            // Relationships
            entity.HasOne(d => d.Institution).WithMany(p => p.TimetableUnits)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("timetable_units_institution_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            // Table Configuration
            entity.ToTable("users", t =>
            {
                // Check constraint for Firstname to allow only letters and spaces
                t.HasCheckConstraint(
                    "CHK_User_Firstname_Valid",
                    "\"firstname\" ~ '^[a-zA-Z ,.''-]+$'"
                );
                
                // Check constraint for Lastname to allow only letters and spaces
                t.HasCheckConstraint(
                    "CHK_User_Lastname_Valid",
                    "\"lastname\" ~ '^[a-zA-Z ,.''-]+$'"
                );
                
                // Check constraint for Login to allow only letters, digits, spaces, and hyphens
                t.HasCheckConstraint(
                    "CHK_User_Login_Valid",
                    "\"login\" ~ '^[\\w -]+$'"
                );

                // Check constraint for BirthDate to ensure it's not in the future
                t.HasCheckConstraint(
                    "CHK_User_BirthDate_Valid",
                    "\"birth_date\" <= NOW()"
                );

                // Check constraint for PhoneNumber (digits and optional formatting characters)
                t.HasCheckConstraint(
                    "CHK_User_PhoneNumber_Valid",
                    "\"phone_number\" ~ '^(\\+\\d{1,2} )?\\(?\\d{3}\\)?[ .-]\\d{3}[ .-]\\d{4}$'"
                );
                
                // Check constraint for HomeAddress to allow only letters, digits, spaces, and hyphens
                t.HasCheckConstraint("CHK_User_HomeAddress_Valid", 
                    "\"home_address\" ~ '^[A-Za-z\\d''\\.\\- \\,]$'");
            });
            
            entity.UseTptMappingStrategy();
            
            entity.HasIndex(e => e.Login, "users_login_key").IsUnique();
            
            // Property Configuration
            entity.Property(e => e.AvatarPath)
                .HasMaxLength(255)
                .HasColumnName("avatar_path");

            entity.Property(e => e.BirthDate)
                .HasColumnName("birth_date");
            
            entity.Property(e => e.Firstname)
                .HasMaxLength(255)
                .HasColumnName("firstname");
            
            entity.Property(e => e.HomeAddress)
                .HasMaxLength(255)
                .HasColumnName("home_address");
            
            entity.Property(e => e.Lastname)
                .HasMaxLength(255)
                .HasColumnName("lastname");
            
            entity.Property(e => e.Login)
                .HasMaxLength(263)
                .HasColumnName("login");
            
            entity.Property(e => e.Password)
                .HasMaxLength(256)
                .HasColumnName("password");
            
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            
            entity.Property(e => e.RoleId)
                .HasColumnName("role_id");
            
            entity.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");
            
            // Relationships
            entity.HasOne(d => d.Role)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("users_role_id_fkey");
            
            entity.HasOne(d => d.Institution)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("users_institution_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
