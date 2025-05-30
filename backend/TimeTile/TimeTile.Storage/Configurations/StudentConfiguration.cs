using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            // Table Configuration
            builder.ToTable("students");

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
