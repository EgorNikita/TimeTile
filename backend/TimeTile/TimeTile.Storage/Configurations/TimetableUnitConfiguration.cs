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
    internal class TimetableUnitConfiguration : IEntityTypeConfiguration<TimetableUnit>
    {
        public void Configure(EntityTypeBuilder<TimetableUnit> builder)
        {
            // Table Configuration
            builder.ToTable("timetable_units", t =>
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

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.InstitutionId, e.Title })
                .HasDatabaseName("timetable_units_institution_title_key")
                .IsUnique();

            builder.HasIndex(e => new { e.InstitutionId, e.Title, e.StartTime, e.EndTime })
                .HasDatabaseName("timetable_units_institution_title_start_end_key")
                .IsUnique();

            // Property Configuration
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.StartTime)
                .HasColumnName("start_time")
                .HasColumnType("time with time zone");

            builder.Property(e => e.EndTime)
                .HasColumnName("end_time")
                .HasColumnType("time with time zone");

            builder.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");

            builder.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            // Relationships
            builder.HasOne(d => d.Institution).WithMany(p => p.TimetableUnits)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("timetable_units_institution_id_fkey");
        }
    }
}
