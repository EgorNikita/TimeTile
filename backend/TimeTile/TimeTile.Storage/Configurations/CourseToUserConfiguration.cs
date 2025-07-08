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
    class CourseToUserConfiguration : IEntityTypeConfiguration<CourseToUser>
    {
        public void Configure(EntityTypeBuilder<CourseToUser> builder)
        {
            // Table and Key Configuration
            builder.ToTable("courses_users", t =>
            {
                t.HasCheckConstraint(
                    "CK_CoursesUsers_OrderNumber_Positive",
                    "\"order_number\" > 0"
                );
            });

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.CourseId, e.UserId, e.DeletedAt })
                .HasDatabaseName("courses_users_course_user_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            builder.HasIndex(e => new { e.UserId, e.OrderNumber, e.DeletedAt })
                .HasDatabaseName("courses_users_user_order_number_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Property Configurations
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.CourseId)
                .HasColumnName("course_id");

            builder.Property(e => e.UserId)
                .HasColumnName("user_id");

            builder.Property(e => e.OrderNumber)
                .HasColumnName("order_number");

            // Relationships
            builder.HasOne(d => d.Course)
                .WithMany(p => p.CoursesToUsers)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_users_course_id_fkey");

            builder.HasOne(d => d.User)
                .WithMany(p => p.CoursesToUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("courses_users_user_id_fkey");
        }
    }
}
