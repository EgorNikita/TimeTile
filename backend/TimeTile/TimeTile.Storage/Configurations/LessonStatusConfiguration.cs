using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class LessonStatusConfiguration : IEntityTypeConfiguration<LessonStatus>
    {
        public void Configure(EntityTypeBuilder<LessonStatus> builder)
        {
            // Table Configuration
            builder.ToTable("lesson_statuses", t =>
            {
                t.HasCheckConstraint(
                    "CHK_LessonStatus_Description_Valid",
                    $"\"description\"  ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns[RegexPatterns.Pattern.Description].PostgresPattern.ToString())}'"
                );
            });

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.Description, e.DeletedAt })
                .HasDatabaseName("lesson_statuses_description_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Property Configuration
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id"); 

            builder.Property(e => e.Description)
                .HasMaxLength(RegexPatterns.Patterns[RegexPatterns.Pattern.Description].MaxLength)
                .HasColumnName("description");
        }
    }
}
