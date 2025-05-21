using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class TermConfiguration : IEntityTypeConfiguration<Term>
    {
        public void Configure(EntityTypeBuilder<Term> builder)
        {
            // Table Configuration
            builder.ToTable("terms", t =>
            {
                t.HasCheckConstraint(
                    "CHK_Term_Title_Valid",
                    "\"title\" ~ '^[\\w -.*+,]+$'"
                );
                t.HasCheckConstraint(
                    "CHK_Term_StartDate_LessThan_EndDate",
                    "\"start_date\" < \"end_date\""
                );
            });

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.InstitutionId, e.Title })
                .HasDatabaseName("terms_institution_title_key")
                .IsUnique();

            builder.HasIndex(e => new { e.StartDate, e.EndDate, e.InstitutionId })
                .HasDatabaseName("terms_institution_start_end_key")
                .IsUnique();

            // Property Configuration
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");

            builder.Property(e => e.Title)
                .HasColumnName("title");

            builder.Property(e => e.StartDate)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("start_date");

            builder.Property(e => e.EndDate)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("end_date");

            // Relationships
            builder.HasOne(d => d.Institution)
                .WithMany(p => p.Terms)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("terms_institution_id_fkey");
        }
    }
}
