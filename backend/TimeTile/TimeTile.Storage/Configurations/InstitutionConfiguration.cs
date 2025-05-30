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
    internal class InstitutionConfiguration : IEntityTypeConfiguration<Institution>
    {
        public void Configure(EntityTypeBuilder<Institution> builder)
        {
            // Table Configuration
            builder.ToTable("institutions", t =>
            {
                // Check constraint for Title to allow only letters, digits, spaces, and special characters
                t.HasCheckConstraint("CHK_Institution_Title_NotEmpty",
                    "\"title\" ~ '^[\\w \\-.*&\"'',\\/\\\\|]+$'");

                // Check constraint for Address to allow only letters, digits, spaces, and special characters
                t.HasCheckConstraint("CHK_Institution_Address_NotEmpty",
                    "\"address\" ~ '^[A-Za-z\\d''\\.\\- \\,]+$'");

                // Check constraint for Email (valid format)
                t.HasCheckConstraint("CHK_Institution_Email_Valid",
                    "\"email\" ~ '^[A-Za-z\\d._%+-]+@[A-Za-z\\d.-]+\\.[A-Za-z]{2,}$'");

                // Check constraint for PhoneNumber (digits and optional formatting characters)
                t.HasCheckConstraint("CHK_Institution_Phone_Valid",
                    "\"phone_number\" ~ '^(\\+\\d{1,2} )?\\(?\\d{3}\\)?[ .-]\\d{3}[ .-]\\d{4}$'");
            });

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.Title, e.DeletedAt })
                .HasDatabaseName("institutions_title_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Property Configurations
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            builder.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");

            builder.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
        }
    }
}
