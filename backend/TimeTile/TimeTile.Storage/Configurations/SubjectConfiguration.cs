using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            // Table Configuration
            builder.ToTable("subjects", t =>
                t.HasCheckConstraint(
                    "CHK_Subject_Title_Valid",
                    $"\"title\" ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns["Title"].PostgresPattern.ToString())}'"
                ));

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.Title, e.InstitutionId, e.DeletedAt })
                .HasDatabaseName("subjects_title_institution_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Property Configuration
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.Title)
                .HasMaxLength(RegexPatterns.Patterns["Title"].MaxLength)
                .HasColumnName("title");

            builder.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");

            // Many-to-Many Configuration
            builder.HasOne(d => d.Institution)
                .WithMany(p => p.Subjects)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("subjects_institution_id_fkey");
        }
    }
}
