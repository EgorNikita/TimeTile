using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            // Table configuration
            builder.ToTable("assignments", t =>
            {
                t.HasCheckConstraint(
                    "CHK_Assignment_Title_Valid",
                    $"\"title\"  ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns[RegexPatterns.Pattern.Title].PostgresPattern.ToString())}'"
                );

                t.HasCheckConstraint(
                    "CHK_Assignment_Description_Valid",
                    $"\"description\"  ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns[RegexPatterns.Pattern.Description].PostgresPattern.ToString())}'"
                );

                t.HasCheckConstraint(
                    "CHK_Assignment_Deadline_Valid",
                    $"\"deadline\"  > \"published_at\""
                );
            });

            builder.HasKey(e => e.Id);

            // Property configurations
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.Title)
                .HasMaxLength(RegexPatterns.Patterns[RegexPatterns.Pattern.Title].MaxLength)
                .HasColumnName("title");

            builder.Property(e => e.Description)
                .HasMaxLength(RegexPatterns.Patterns[RegexPatterns.Pattern.Description].MaxLength)
                .HasColumnName("description");

            builder.Property(e => e.PublishedAt)
                .HasColumnName("published_at");

            builder.Property(e => e.Deadline)
                .HasColumnName("deadline");

            builder.Property(e => e.UploadAfterDeadline)
                .HasDefaultValue(true)
                .HasColumnName("upload_after_deadline");
        }
    }
}
