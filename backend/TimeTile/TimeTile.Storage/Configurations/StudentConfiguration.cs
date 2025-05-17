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
    internal class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            // Table Configuration
            builder.ToTable("students");

            builder.HasIndex(e => new { e.Id, e.GroupId })
                .HasDatabaseName("students_id_group_key")
                .IsUnique();

            // Property Configuration
            builder.Property(e => e.GroupId)
                .HasColumnName("group_id");

            // Relationship Configuration
            builder.HasOne(d => d.Group).WithMany(p => p.Students)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("students_group_id_fkey");
        }
    }
}
