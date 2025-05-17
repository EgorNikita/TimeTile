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

            builder.HasIndex(e => e.Description, "permissions_description_key").IsUnique();

            // Property Configuration
            builder.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");

            builder.HasMany(p => p.Roles)  // A Permission can have many Roles
                .WithMany(r => r.Permissions)  // A Role can have many Permissions
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermissions",  // Name of the join table
                    j => j
                        .HasOne<Role>()  // The join table has one Role
                        .WithMany()  // A Role can be related to many Permissions
                        .HasForeignKey("role_id")  // Foreign key for Role
                        .OnDelete(DeleteBehavior.NoAction)  // No action on delete
                        .HasConstraintName("role_permissions_role_id_fkey"),  // Foreign key constraint name
                    j => j
                        .HasOne<Permission>()  // The join table has one Permission
                        .WithMany()  // A Permission can be related to many Roles
                        .HasForeignKey("permission_id")  // Foreign key for Permission
                        .OnDelete(DeleteBehavior.NoAction)  // No action on delete
                        .HasConstraintName("role_permissions_permission_id_fkey"),  // Foreign key constraint name
                    j =>
                    {
                        j.ToTable("roles_permissions");  // Name of the join table
                        j.HasKey("role_id", "permission_id")  // Composite primary key
                            .HasName("role_permissions_pkey");  // Name of the composite primary key
                    });
        }
    }
}
