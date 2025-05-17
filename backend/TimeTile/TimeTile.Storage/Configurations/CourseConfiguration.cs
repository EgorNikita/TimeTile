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
    internal class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            // Table configuration
            builder.ToTable("courses", t =>
                t.HasCheckConstraint(
                    "CHK_Course_Title_Valid",
                    "\"title\"  ~ '^[\\w -.*+,]+$'"
                ));

            builder.HasIndex(e => new { e.Title, e.SubjectId, e.TeacherId, e.InstitutionId, e.TermId })
                .IsUnique()
                .HasDatabaseName("courses_title_subject_teacher_institution_term_key");

            // Property configurations
            builder.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            builder.Property(e => e.IsAdvanced)
                .HasDefaultValue(false)
                .HasColumnName("is_advanced");

            builder.Property(e => e.SubjectId)
                .HasColumnName("subject_id");

            builder.Property(e => e.TeacherId)
                .HasColumnName("teacher_id");

            builder.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");

            builder.Property(e => e.TermId)
                .HasColumnName("term_id");

            // Relationships
            builder.HasOne(d => d.Subject)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_subject_id_fkey");

            builder.HasOne(d => d.Teacher)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_teacher_id_fkey");

            builder.HasOne(d => d.Institution)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_institution_id_fkey");

            builder.HasOne(d => d.Term)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.TermId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_term_id_fkey");
        }
    }
}
