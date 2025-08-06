using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            // Table configuration
            builder.ToTable("messages", t =>
            {
                t.HasCheckConstraint(
                    "CHK_Message_Sent_At_Valid",
                    $"\"sent_at\" < NOW()"
                );

                t.HasCheckConstraint(
                    "CHK_Message_Edited_At_Valid",
                    $"\"edited_at\" IS NULL OR \"edited_at\" > \"sent_at\""
                );
            });

            builder.HasKey(e => e.Id);

            // Property configurations
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.UserId)
                .HasColumnName("user_id");

            builder.Property(e => e.CourseId)
                .HasColumnName("course_id");

            builder.Property(e => e.Content)
                .HasColumnName("content")
                .IsRequired(false);

            builder.Property(e => e.SentAt)
                .HasColumnName("sent_at");

            builder.Property(e => e.EditedAt)
                .HasColumnName("edited_at")
                .IsRequired(false);

            // Define relationships
            builder.HasOne(d => d.User)
                .WithMany(p => p.Messages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("messages_user_id_fkey");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.Messages)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("messages_classroom_type_id_fkey");
        }
    }
}
