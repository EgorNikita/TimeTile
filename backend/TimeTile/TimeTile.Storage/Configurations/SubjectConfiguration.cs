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
    internal class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            // Table Configuration
            builder.ToTable("subjects", t =>
                t.HasCheckConstraint(
                    "CHK_Subject_Title_Valid",
                    "\"title\" ~ '^[\\w -]+$'"
                ));

            builder.HasIndex(e => e.Title, "subjects_title_key").IsUnique();

            // Property Configuration
            builder.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            // Many-to-Many Configuration
            builder.HasMany(s => s.Institutions)
                .WithMany(i => i.Subjects)
                .UsingEntity<Dictionary<string, object>>(
                    "SubjectInstitution",
                    j => j
                        .HasOne<Institution>()
                        .WithMany()
                        .HasForeignKey("institution_id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("subjects_institutions_institution_id_fkey"),
                    j => j
                        .HasOne<Subject>()
                        .WithMany()
                        .HasForeignKey("subject_id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("subjects_institutions_subject_id_fkey"),
                    j =>
                    {
                        j.ToTable("subjects_institutions");  // Name of the join table
                        j.HasKey("institution_id", "subject_id")  // Composite primary key
                            .HasName("subjects_institutions_pkey");  // Name of the composite primary key
                    });
        }
    }
}
