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
    internal class LessonStatusConfiguration : IEntityTypeConfiguration<LessonStatus>
    {
        public void Configure(EntityTypeBuilder<LessonStatus> builder)
        {
            // Table Configuration
            builder.ToTable("lesson_statuses", t =>
            {
                t.HasCheckConstraint(
                    "CHK_LessonStatus_Description_Valid",
                    "\"description\"  ~ '^[a-zA-Z\\d ]+$'"
                );
            });

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.Description, e.InstitutionId, e.DeletedAt })
                .HasDatabaseName("lesson_statuses_description_institution_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Property Configuration
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id"); 

            builder.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");

            builder.Property(e => e.ArgbColor)
                .HasColumnName("argb_color");

            builder.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");

            // Many-to-Many Configuration
            builder.HasOne(e => e.Institution)
                .WithMany(e => e.LessonStatuses)
                .HasForeignKey(e => e.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lesson_statuses_institution_id_fkey");
        }
    }
}
