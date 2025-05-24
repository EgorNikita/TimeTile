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
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Table Configuration
            builder.ToTable("users", t =>
            {
                // Check constraint for Firstname to allow only letters and spaces
                t.HasCheckConstraint(
                    "CHK_User_Firstname_Valid",
                    "\"firstname\" ~ '^[a-zA-Z ,.''-]+$'"
                );

                // Check constraint for Lastname to allow only letters and spaces
                t.HasCheckConstraint(
                    "CHK_User_Lastname_Valid",
                    "\"lastname\" ~ '^[a-zA-Z ,.''-]+$'"
                );

                // Check constraint for Login to allow only letters, digits, spaces, and hyphens
                t.HasCheckConstraint(
                    "CHK_User_Login_Valid",
                    "\"login\" ~ '^[\\w -]+$'"
                );

                // Check constraint for BirthDate to ensure it's not in the future
                t.HasCheckConstraint(
                    "CHK_User_BirthDate_Valid",
                    "\"birth_date\" <= NOW()"
                );

                // Check constraint for PhoneNumber (digits and optional formatting characters)
                t.HasCheckConstraint(
                    "CHK_User_PhoneNumber_Valid",
                    "\"phone_number\" ~ '^(\\+\\d{1,2} )?\\(?\\d{3}\\)?[ .-]\\d{3}[ .-]\\d{4}$'"
                );

                // Check constraint for HomeAddress to allow only letters, digits, spaces, and hyphens
                t.HasCheckConstraint("CHK_User_HomeAddress_Valid",
                    "\"home_address\" ~ '^[A-Za-z\\d''\\.\\- \\,]$'");
            });

            builder.HasKey(e => e.Id);

            builder.UseTptMappingStrategy();

            builder.HasIndex(e => new { e.Login, e.DeletedAt })
                .HasDatabaseName("users_login_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Property Configuration
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.AvatarPath)
                .HasMaxLength(255)
                .HasColumnName("avatar_path");

            builder.Property(e => e.BirthDate)
                .HasColumnName("birth_date");

            builder.Property(e => e.Firstname)
                .HasMaxLength(255)
                .HasColumnName("firstname");

            builder.Property(e => e.HomeAddress)
                .HasMaxLength(255)
                .HasColumnName("home_address");

            builder.Property(e => e.Lastname)
                .HasMaxLength(255)
                .HasColumnName("lastname");

            builder.Property(e => e.Login)
                .HasMaxLength(263)
                .HasColumnName("login");

            builder.Property(e => e.PasswordHash)
                .HasMaxLength(256)
                .HasColumnName("password_hash");

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");

            builder.Property(e => e.RoleId)
                .HasColumnName("role_id");

            builder.Property(e => e.InstitutionId)
                .HasColumnName("institution_id");

            // Relationships
            builder.HasOne(d => d.Role)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("users_role_id_fkey");

            builder.HasOne(d => d.Institution)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("users_institution_id_fkey");
        }
    }
}
