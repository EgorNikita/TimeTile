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
    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Table Configuration
            builder.ToTable("roles", t =>
                t.HasCheckConstraint(
                    "CHK_Role_Title_Valid",
                    "\"title\"  ~ '^[\\w -]+$'"
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
                .HasMaxLength(255)
                .HasColumnName("title");

            builder.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");

            // Relationships
            builder.HasOne(d => d.Institution)
                .WithMany(p => p.Roles)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("roles_institution_id_fkey");
        }
    }
}
