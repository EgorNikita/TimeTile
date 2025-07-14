using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            // Table Configuration
            builder.ToTable("lessons");

            builder.HasKey(e => e.Id);

            // Property Configurations
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.ClassroomId)
                .HasColumnName("classroom_id");

            builder.Property(e => e.CourseId)
                .HasColumnName("course_id");

            builder.Property(e => e.Date)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("date");

            builder.Property(e => e.Description)
                .HasMaxLength(RegexPatterns.Patterns[RegexPatterns.Pattern.Description].MaxLength)
                .HasColumnName("description");

            builder.Property(e => e.AssignmentId)
                .HasColumnName("assignment_id")
                .IsRequired(false);

            builder.Property(e => e.LessonStatusId)
                .HasColumnName("lesson_status_id");

            // Relationship Configurations
            builder.HasOne(d => d.Classroom)
                .WithMany(p => p.Lessons)
                .HasForeignKey(d => d.ClassroomId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_classroom_id_fkey");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.Lessons)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_course_id_fkey");

            builder.HasOne(d => d.LessonStatus)
                .WithMany(p => p.Lessons)
                .HasForeignKey(d => d.LessonStatusId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_lesson_status_id_fkey");

            builder.HasOne(d => d.Assignment)
                .WithOne(p => p.Lesson)
                .HasForeignKey<Lesson>(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_assignment_id_fkey");
        }
    }
}
