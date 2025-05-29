using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class LessonToStudentConfiguration : IEntityTypeConfiguration<LessonToStudent>
    {
        public void Configure(EntityTypeBuilder<LessonToStudent> builder)
        {
            // Map to table and create index
            builder.ToTable("lessons_students", t =>
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

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.LessonId, e.StudentId, e.DeletedAt })
                .HasDatabaseName("lessons_students_lesson_student_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Define properties with column names
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.LessonId)
                .HasColumnName("lesson_id");

            builder.Property(e => e.StudentId)
                .HasColumnName("student_id");

            builder.Property(e => e.CameAt)
                .HasColumnName("came_at")
                .HasColumnType("time with time zone")
                .IsRequired(false);

            builder.Property(e => e.LeftAt)
                .HasColumnName("left_at")
                .HasColumnType("time with time zone")
                .IsRequired(false);

            builder.Property(e => e.ClassworkGradeId)
                .HasColumnName("classwork_grade_id")
                .IsRequired(false);

            builder.Property(e => e.HomeworkGradeId)
                .HasColumnName("homework_grade_id")
                .IsRequired(false);

            // Define relationships
            builder.HasOne(d => d.ClassworkGrade)
                .WithOne(p => p.LessonToStudentClasswork)
                .HasForeignKey<LessonToStudent>(d => d.ClassworkGradeId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_students_classwork_grade_id_fkey");

            builder.HasOne(d => d.HomeworkGrade)
                .WithOne(p => p.LessonToStudentHomework)
                .HasForeignKey<LessonToStudent>(d => d.HomeworkGradeId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_students_homework_grade_id_fkey");

            builder.HasOne(d => d.Lesson)
                .WithMany(p => p.LessonsToStudents)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_students_lesson_id_fkey");

            builder.HasOne(d => d.Student)
                .WithMany(p => p.LessonsToStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_students_student_id_fkey");
        }
    }
}
