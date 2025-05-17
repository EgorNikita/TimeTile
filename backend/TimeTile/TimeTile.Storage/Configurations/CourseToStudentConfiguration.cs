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
    internal class CourseToStudentConfiguration : IEntityTypeConfiguration<CourseToStudent>
    {
        public void Configure(EntityTypeBuilder<CourseToStudent> builder)
        {
            // Table and Key Configuration
            builder.ToTable("courses_students", t =>
                t.HasCheckConstraint(
                    "CK_CoursesStudents_HasExam_ExamGrade",
                    "\"has_exam\" = FALSE OR \"exam_grade_id\" IS NOT NULL"
                ));

            builder.HasKey(e => e.Id).HasName("courses_students_pkey");

            builder.HasIndex(e => new { e.CourseId, e.StudentId }, "courses_students_course_id_student_id_key")
                .IsUnique();

            // Property Configurations
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.CourseId).HasColumnName("course_id");
            builder.Property(e => e.StudentId).HasColumnName("student_id");
            builder.Property(e => e.ExamGradeId)
                .HasColumnName("exam_grade_id")
                .IsRequired(false);
            builder.Property(e => e.HasExam)
                .HasColumnName("has_exam");

            // Relationships
            builder.HasOne(d => d.Course)
                .WithMany(p => p.CoursesToStudents)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_students_course_id_fkey");

            builder.HasOne(d => d.Student)
                .WithMany(p => p.CoursesToStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_students_student_id_fkey");

            builder.HasOne(d => d.ExamGrade)
                .WithOne(p => p.CourseToStudent)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_students_exam_grade_id_fkey");
        }
    }
}
