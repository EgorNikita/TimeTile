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
    internal class AuditableEntityConfiguration : IEntityTypeConfiguration<AuditableEntity>
    {
        public void Configure(EntityTypeBuilder<AuditableEntity> builder)
        {
            builder.HasQueryFilter(e => e.DeletedAt == null);


            builder.ToTable("auditable_entities");
            builder.HasKey(e => e.Id);

            // Use TPT inheritance mapping strategy
            builder.UseTptMappingStrategy();

            // Property configurations
            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at")
                .IsRequired();

            builder.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deleted_at")
                .IsRequired(false); // Nullable for soft deletion
        }
    }
}
