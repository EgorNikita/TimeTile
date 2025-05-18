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
    public class TeacherToSubjectConfiguration : IEntityTypeConfiguration<TeacherToSubject>
    {
        public void Configure(EntityTypeBuilder<TeacherToSubject> builder)
        {
            // Map to table and create index
            builder.ToTable("teachers_subjects");

            builder.HasIndex(e => new { e.TeacherId, e.SubjectId }, "teachers_subjects_teacher_id_subject_id_key")
                .IsUnique();

            // Define properties with column names
            builder.Property(e => e.TeacherId)
                .HasColumnName("teacher_id");
            builder.Property(e => e.SubjectId)
                .HasColumnName("subject_id");

            // Define relationships
            builder.HasOne(d => d.Teacher)
                .WithMany(p => p.TeacherToSubjects)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("teachers_subjects_teacher_id_fkey");

            builder.HasOne(d => d.Subject)
                .WithMany(p => p.TeachersToSubject)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("teachers_subjects_subject_id_fkey");
        }
    }
}
