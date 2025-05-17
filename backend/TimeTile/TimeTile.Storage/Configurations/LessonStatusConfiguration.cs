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
                t.HasCheckConstraint(
                    "CHK_LessonStatus_Description_Valid",
                    "\"description\"  ~ '^[a-zA-Z\\d ]+$'"
                ));

            builder.HasIndex(e => e.Description)
                .HasDatabaseName("lesson_statuses_description_key")
                .IsUnique();

            // Property Configuration
            builder.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");

            // Many-to-Many Configuration
            builder.HasMany(e => e.Institutions)
                .WithMany(e => e.LessonStatuses)
                .UsingEntity<Dictionary<string, object>>(
                    "LessonStatusInstitution",
                    j => j
                        .HasOne<Institution>()
                        .WithMany()
                        .HasForeignKey("institution_id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("lesson_status_institution_institution_id_fkey"),
                    j => j
                        .HasOne<LessonStatus>()
                        .WithMany()
                        .HasForeignKey("lesson_status_id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("lesson_status_institution_lesson_status_id_fkey"),
                    j =>
                    {
                        j.ToTable("lesson_statuses_institutions");
                        j.HasKey("lesson_status_id", "institution_id")
                            .HasName("lesson_status_institution_pkey");
                    });
        }
    }
}
