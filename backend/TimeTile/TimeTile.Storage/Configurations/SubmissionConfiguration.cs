using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
    {
        public void Configure(EntityTypeBuilder<Submission> builder)
        {
            // Table and Key Configuration
            builder.ToTable("submissions", t =>
            {
                t.HasCheckConstraint(
                    "CHK_Submission_Grade_Valid",
                    $"\"grade_id\" IS NULL OR LOWER(\"status\") = LOWER('{nameof(SubmissionStatus.Accepted)}')"
                );

                string[] statuses = Enum.GetNames(typeof(SubmissionStatus))
                    .Select(e => '\'' + e.ToLower() + '\'')
                    .ToArray();

                string statusesString = string.Join(", ", statuses);

                t.HasCheckConstraint(
                    "CHK_Submission_Status_Valid",
                    $"LOWER(\"status\") IN ({statusesString})"
                );
            });

            builder.HasKey(e => e.Id);

            // Property Configurations
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.AssignmentId)
                .HasColumnName("assignment_id");

            builder.Property(e => e.StudentId)
                .HasColumnName("student_id");

            builder.Property(e => e.GradeId)
                .HasColumnName("grade_id")
                .IsRequired(false);

            builder.Property(e => e.Status)
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<SubmissionStatus>(v, true)
                )
                .HasColumnName("status")
                .HasConversion<string>();

            builder.Property(e => e.StudentNote)
                .HasColumnName("student_note");

            builder.Property(e => e.Feedback)
                .HasColumnName("feedback");

            // Relationships
            builder.HasOne(d => d.Assignment)
                .WithMany(p => p.Submissions)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("submissions_assignment_id_fkey");

            builder.HasOne(d => d.Student)
                .WithMany(p => p.Submissions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("submissions_student_id_fkey");

            builder.HasOne(d => d.Grade)
                .WithOne(p => p.Submission)
                .HasForeignKey<Submission>(d => d.GradeId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("submissions_grade_id_fkey");
        }
    }
}
