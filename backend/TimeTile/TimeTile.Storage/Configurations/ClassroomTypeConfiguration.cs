using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class ClassroomTypeConfiguration : IEntityTypeConfiguration<ClassroomType>
    {
        public void Configure(EntityTypeBuilder<ClassroomType> builder)
        {
            // Table configuration
            builder.ToTable("classroom_types");

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.Description, e.InstitutionId, e.DeletedAt })
                .HasDatabaseName("classroom_types_description_institution_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Property configurations
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(RegexPatterns.Patterns[RegexPatterns.Pattern.Description].MaxLength);

            builder.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");

            builder.Property(e => e.IconId)
                .HasColumnName("icon_id")
                .IsRequired(false);

            // Relationships
            builder.HasOne(d => d.Institution)
                .WithMany(p => p.ClassroomTypes)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("classroom_types_institution_id_fkey");

            builder.HasOne(d => d.Icon)
                .WithOne(p => p.ClassroomType)
                .HasForeignKey<ClassroomType>(d => d.IconId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("classroom_types_icon_id_fkey");
        }
    }
}
