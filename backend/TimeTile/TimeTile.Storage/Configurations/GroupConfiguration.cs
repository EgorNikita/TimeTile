using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            // Table and Index Configuration
            builder.ToTable("groups", t =>
                t.HasCheckConstraint(
                    "CHK_Group_Title_Valid",
                    $"\"title\"  ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns[RegexPatterns.Pattern.Title].PostgresPattern.ToString())}'"
                ));

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.InstitutionId, e.Title, e.DeletedAt })
                .HasDatabaseName("groups_institution_title_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Property Configurations
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");

            builder.Property(e => e.Title)
                .HasMaxLength(RegexPatterns.Patterns[RegexPatterns.Pattern.Title].MaxLength)
                .HasColumnName("title");

            builder.Property(e => e.AvatarId)
                .HasColumnName("avatar_id")
                .IsRequired(false);

            // Relationships
            builder.HasOne(d => d.Institution)
                .WithMany(p => p.Groups)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("groups_institution_id_fkey");

            builder.HasOne(d => d.Avatar)
                .WithOne(p => p.Group)
                .HasForeignKey<Group>(d => d.AvatarId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("groups_avatar_id_fkey");
        }
    }
}
