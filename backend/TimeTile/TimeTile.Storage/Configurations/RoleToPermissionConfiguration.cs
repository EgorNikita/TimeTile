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
    internal class RoleToPermissionConfiguration : IEntityTypeConfiguration<RoleToPermission>
    {
        public void Configure(EntityTypeBuilder<RoleToPermission> builder)
        {
            // Map to table and create index
            builder.ToTable("roles_permissions");

            builder.HasIndex(e => new { e.RoleId, e.PermissionId }, "roles_permissions_role_id_permission_id_key")
                .IsUnique();

            // Define properties with column names
            builder.Property(e => e.RoleId)
                .HasColumnName("role_id");
            builder.Property(e => e.PermissionId)
                .HasColumnName("permission_id");

            // Define relationships
            builder.HasOne(d => d.Role)
                .WithMany(p => p.RoleToPermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("roles_permissions_role_id_fkey");

            builder.HasOne(d => d.Permission)
                .WithMany(p => p.RolesToPermission)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("roles_permissions_permission_id_fkey");
        }
    }
}
