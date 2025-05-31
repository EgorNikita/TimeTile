using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Table Configuration
            builder.ToTable("roles", t =>
                t.HasCheckConstraint(
                    "CHK_Role_Title_Valid",
                    $"\"title\"  ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns[RegexPatterns.Pattern.Title].PostgresPattern.ToString())}'"
                ));

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.InstitutionId, e.Title, e.DeletedAt })
                .HasDatabaseName("roles_title_institution_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Property Configuration
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.Title)
                .HasMaxLength(RegexPatterns.Patterns[RegexPatterns.Pattern.Title].MaxLength)
                .HasColumnName("title");

            builder.Property(e => e.InstitutionId)
                .HasColumnName("institution_id")
                .IsRequired(false);

            // Relationships
            builder.HasOne(d => d.Institution)
                .WithMany(p => p.Roles)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("roles_institution_id_fkey");
        }
    }
}
