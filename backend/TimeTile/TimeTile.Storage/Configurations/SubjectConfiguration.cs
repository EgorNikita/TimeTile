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
