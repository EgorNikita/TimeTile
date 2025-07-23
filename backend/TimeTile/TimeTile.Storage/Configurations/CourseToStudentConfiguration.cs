using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class CourseToStudentConfiguration : IEntityTypeConfiguration<CourseToStudent>
    {
        public void Configure(EntityTypeBuilder<CourseToStudent> builder)
        {
            // Table and Key Configuration
            builder.ToTable("courses_students", t =>
            {
                t.HasCheckConstraint(
                    "CK_CoursesStudents_PositionX_Positive",
                    "\"position_x\" >= 0"
                );
                t.HasCheckConstraint(
                    "CK_CoursesStudents_PositionY_Positive",
                    "\"position_y\" >= 0"
                );
            });

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.CourseId, e.StudentId, e.DeletedAt })
                .HasDatabaseName("courses_students_course_student_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Property Configurations
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            builder.Property(e => e.CourseId).HasColumnName("course_id");
            builder.Property(e => e.StudentId).HasColumnName("student_id");
            builder.Property(e => e.GradeId)
                .HasColumnName("grade_id")
                .IsRequired(false);
            builder.Property(e => e.PositionX).HasColumnName("position_x");
            builder.Property(e => e.PositionY).HasColumnName("position_y");

            // Relationships
            builder.HasOne(d => d.Course)
                .WithMany(p => p.CoursesToStudents)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_students_course_id_fkey");

            builder.HasOne(d => d.Student)
                .WithMany(p => p.CoursesToStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_students_student_id_fkey");

            builder.HasOne(d => d.Grade)
                .WithOne(p => p.CourseToStudent)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_students_exam_grade_id_fkey");
        }
    }
}
