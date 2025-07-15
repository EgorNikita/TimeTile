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
    internal class MessageToFileConfiguration : IEntityTypeConfiguration<MessageToFile>
    {
        public void Configure(EntityTypeBuilder<MessageToFile> builder)
        {
            // Map to table and create index
            builder.ToTable("messages_files");

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.MessageId, e.FileId, e.DeletedAt })
                .HasDatabaseName("messages_files_message_file_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Define properties with column names
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.MessageId)
                .HasColumnName("message_id");

            builder.Property(e => e.FileId)
                .HasColumnName("file_id");

            // Define relationships
            builder.HasOne(d => d.Message)
                .WithMany(p => p.MessageToFiles)
                .HasForeignKey(d => d.MessageId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("messages_files_message_id_fkey");

            builder.HasOne(d => d.File)
                .WithMany(p => p.MessagesToFile)
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("messages_files_file_id_fkey");
        }
    }
}
