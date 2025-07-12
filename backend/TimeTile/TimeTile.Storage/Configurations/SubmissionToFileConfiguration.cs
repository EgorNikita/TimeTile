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
    internal class SubmissionToFileConfiguration : IEntityTypeConfiguration<SubmissionToFile>
    {
        public void Configure(EntityTypeBuilder<SubmissionToFile> builder)
        {
            // Map to table and create index
            builder.ToTable("submissions_files");

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.SubmissionId, e.FileId, e.DeletedAt })
                .HasDatabaseName("submissions_files_submission_file_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Define properties with column names
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.SubmissionId)
                .HasColumnName("submission_id");

            builder.Property(e => e.FileId)
                .HasColumnName("file_id");

            // Define relationships
            builder.HasOne(d => d.Submission)
                .WithMany(p => p.SubmissionToFiles)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("submissions_files_submission_id_fkey");

            builder.HasOne(d => d.File)
                .WithOne(p => p.SubmissionToFile)
                .HasForeignKey<SubmissionToFile>(d => d.FileId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("submissions_files_file_id_fkey");
        }
    }
}
