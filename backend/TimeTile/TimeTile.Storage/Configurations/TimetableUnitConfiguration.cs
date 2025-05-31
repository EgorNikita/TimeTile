using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Common.Regex;
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
                    $"\"title\" ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns[RegexPatterns.Pattern.Title].PostgresPattern.ToString())}'"
                );
                t.HasCheckConstraint(
                    "CHK_TimetableUnit_StartTime_LessThan_EndTime",
                    "\"start_time\" < \"end_time\""
                );
            });

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.InstitutionId, e.Title, e.DeletedAt })
                .HasDatabaseName("timetable_units_institution_title_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            builder.HasIndex(e => new { e.InstitutionId, e.StartTime, e.EndTime, e.DeletedAt })
                .HasDatabaseName("timetable_units_institution_start_end_deleted_at_key")
                .AreNullsDistinct(false)
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
                .HasMaxLength(RegexPatterns.Patterns[RegexPatterns.Pattern.Title].MaxLength)
                .HasColumnName("title");

            // Relationships
            builder.HasOne(d => d.Institution).WithMany(p => p.TimetableUnits)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("timetable_units_institution_id_fkey");
        }
    }
}
