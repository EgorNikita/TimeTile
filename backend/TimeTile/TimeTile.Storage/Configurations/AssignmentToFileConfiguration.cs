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
    internal class AssignmentToFileConfiguration : IEntityTypeConfiguration<AssignmentToFile>
    {
        public void Configure(EntityTypeBuilder<AssignmentToFile> builder)
        {
            // Map to table and create index
            builder.ToTable("assignments_files");

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.AssignmentId, e.FileId, e.DeletedAt })
                .HasDatabaseName("assignments_files_assignment_file_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Define properties with column names
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.AssignmentId)
                .HasColumnName("assignment_id");

            builder.Property(e => e.FileId)
                .HasColumnName("file_id");

            // Define relationships
            builder.HasOne(d => d.Assignment)
                .WithMany(p => p.AssignmentToFiles)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("assignments_files_assignment_id_fkey");

            builder.HasOne(d => d.File)
                .WithOne(p => p.AssignmentToFile)
                .HasForeignKey<AssignmentToFile>(d => d.FileId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("assignments_files_file_id_fkey");
        }
    }
}
