using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Common.Regex;
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
                    $"\"title\" ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns["Title"].PostgresPattern.ToString())}'");

                // Check constraint for Address to allow only letters, digits, spaces, and special characters
                t.HasCheckConstraint("CHK_Institution_Address_NotEmpty",
                    $"\"address\" ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns["Address"].PostgresPattern.ToString())}'");

                // Check constraint for Email (valid format)
                t.HasCheckConstraint("CHK_Institution_Email_Valid",
                    $"\"email\" ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns["Email"].PostgresPattern.ToString())}'");

                // Check constraint for PhoneNumber E.164   
                t.HasCheckConstraint("CHK_Institution_Phone_Valid",
                    $"\"phone_number\" ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns["PhoneE164"].PostgresPattern.ToString())}'");
                
                t.HasCheckConstraint("CHK_Institution_Domain_Valid",
                    $"\"domain\" ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns["Domain"].PostgresPattern.ToString())}'");
            });

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.Title, e.DeletedAt })
                .HasDatabaseName("institutions_title_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            builder.HasIndex(e => new { e.Domain, e.DeletedAt })
                .HasDatabaseName("institutions_domain_deleted_at_constraint")
                .AreNullsDistinct(false)
                .IsUnique();
            
            // Property Configurations
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.Title)
                .HasMaxLength(RegexPatterns.Patterns["Title"].MaxLength)
                .HasColumnName("title");

            builder.Property(e => e.Address)
                .HasMaxLength(RegexPatterns.Patterns["Address"].MaxLength)
                .HasColumnName("address");

            builder.Property(e => e.Email)
                .HasMaxLength(RegexPatterns.Patterns["Email"].MaxLength)
                .HasColumnName("email");

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(RegexPatterns.Patterns["PhoneE164"].MaxLength)
                .HasColumnName("phone_number");
            
            builder.Property(e => e.Domain)
                .HasMaxLength(RegexPatterns.Patterns["Domain"].MaxLength)
                .HasColumnName("domain");
        }
    }
}
