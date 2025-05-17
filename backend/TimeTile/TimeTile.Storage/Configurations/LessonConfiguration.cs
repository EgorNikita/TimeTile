using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            // Table Configuration
            builder.ToTable("lessons");

            builder.HasIndex(e => new { e.CourseId, e.TimetableUnitId, e.Date })
                .HasDatabaseName("lessons_course_timetable_date_key")
                .IsUnique();

            // Property Configurations
            builder.Property(e => e.ClassroomId)
                .HasColumnName("classroom_id");

            builder.Property(e => e.CourseId)
                .HasColumnName("course_id");

            builder.Property(e => e.Date)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("date");

            builder.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");

            builder.Property(e => e.HomeworkDescription)
                .HasMaxLength(255)
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
