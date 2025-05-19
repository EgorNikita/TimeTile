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
    internal class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            // Table Configuration
            builder.ToTable("permissions", t =>
                t.HasCheckConstraint(
                    "CHK_Permission_Description_Valid",
                    "\"description\"  ~ '^[\\w -]+$'"
                ));

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.Description, "permissions_description_key").IsUnique();

            // Property Configuration
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
        }
    }
}
