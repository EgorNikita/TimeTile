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
    internal class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            // Table and Index Configuration
            builder.ToTable("groups", t =>
                t.HasCheckConstraint(
                    "CHK_Group_Title_Valid",
                    "\"title\"  ~ '^[\\w -.*]+$'"
                ));

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.InstitutionId, e.Title })
                .HasDatabaseName("groups_institution_title_key")
                .IsUnique();

            // Property Configurations
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");

            builder.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            // Relationships
            builder.HasOne(d => d.Institution)
                .WithMany(p => p.Groups)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("groups_institution_id_fkey");
        }
    }
}
