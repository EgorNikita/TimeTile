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

            builder.HasIndex(e => new { e.CourseId, e.TimetableUnitId, e.Date, e.DeletedAt })
                .HasDatabaseName("lessons_course_timetable_date_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

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
                .HasMaxLength(RegexPatterns.Patterns["Description"].MaxLength)
                .HasColumnName("description");

            builder.Property(e => e.HomeworkDescription)
                .HasMaxLength(RegexPatterns.Patterns["Description"].MaxLength)
                .HasColumnName("homework_description");

            builder.Property(e => e.LessonStatusId)
                .HasColumnName("lesson_status_id");

            builder.Property(e => e.TimetableUnitId)
                .HasColumnName("timetable_unit_id");

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

            builder.HasOne(d => d.TimetableUnit)
                .WithMany(p => p.Lessons)
                .HasForeignKey(d => d.TimetableUnitId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_timetable_unit_id_fkey");
        }
    }
}
