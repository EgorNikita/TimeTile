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
    internal class LessonToTimetableUnitConfiguration : IEntityTypeConfiguration<LessonToTimetableUnit>
    {
        public void Configure(EntityTypeBuilder<LessonToTimetableUnit> builder)
        {
            // Map to table and create index
            builder.ToTable("lessons_timetable_units");

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.LessonId, e.TimetableUnitId, e.DeletedAt })
                .HasDatabaseName("lessons_timetable_units_lesson_timetable_unit_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Define properties with column names
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.LessonId)
                .HasColumnName("lesson_id");

            builder.Property(e => e.TimetableUnitId)
                .HasColumnName("timetable_unit_id");

            // Define relationships
            builder.HasOne(d => d.Lesson)
                .WithMany(p => p.LessonToTimetableUnits)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_timetable_units_lesson_id_fkey");

            builder.HasOne(d => d.TimetableUnit)
                .WithMany(p => p.LessonsToTimetableUnit)
                .HasForeignKey(d => d.TimetableUnitId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lessons_timetable_units_timetable_unit_id_fkey");
        }
    }
}
