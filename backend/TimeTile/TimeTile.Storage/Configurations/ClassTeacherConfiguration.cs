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
    internal class ClassTeacherConfiguration : IEntityTypeConfiguration<ClassTeacher>
    {
        public void Configure(EntityTypeBuilder<ClassTeacher> builder)
        {
            // Table configuration
            builder.ToTable("class_teachers");

            // Property configurations
            builder.Property(e => e.GroupId)
                .HasColumnName("group_id");

            // Relationships
            builder.HasOne(d => d.Group)
                .WithMany(p => p.ClassTeachers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("class_teachers_group_id_fkey");
        }
    }
}
