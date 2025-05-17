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
    internal class ClassroomConfiguration : IEntityTypeConfiguration<Classroom>
    {
        public void Configure(EntityTypeBuilder<Classroom> builder)
        {
            // Table configuration
            builder.ToTable("classrooms", t =>
                t.HasCheckConstraint(
                    "CHK_Classroom_Title_Valid",
                    "\"title\" ~ '^[a-zA-Z \\d-]+$'"
                ));

            // Unique index for InstitutionId and Title
            builder.HasIndex(e => new { e.InstitutionId, e.Title })
                .IsUnique()
                .HasDatabaseName("classrooms_institution_title_key");

            // Property configurations
            builder.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");

            builder.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            builder.HasOne(d => d.Institution)
                .WithMany(p => p.Classrooms)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("classrooms_institution_id_fkey");
        }
    }
}
