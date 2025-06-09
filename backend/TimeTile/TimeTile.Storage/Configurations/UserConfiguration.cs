using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Common.Regex;
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
                    $"\"firstname\" ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns[RegexPatterns.Pattern.Name].PostgresPattern.ToString())}'"
                );

                // Check constraint for Lastname to allow only letters and spaces
                t.HasCheckConstraint(
                    "CHK_User_Lastname_Valid",
                    $"\"lastname\" ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns[RegexPatterns.Pattern.Name].PostgresPattern.ToString())}'"
                );

                // Check constraint for Login to allow only letters, digits, spaces, and hyphens
                t.HasCheckConstraint(
                    "CHK_User_Login_Valid",
                    $"\"login\" ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns[RegexPatterns.Pattern.Email].PostgresPattern.ToString())}'"
                );
                
                // Check constraint for BirthDate to ensure it's not in the future
                t.HasCheckConstraint(
                    "CHK_User_BirthDate_Valid",
                    "\"birth_date\" <= NOW()"
                );

                // Check constraint for PhoneNumber E.164
                t.HasCheckConstraint(
                    "CHK_User_PhoneNumber_Valid",
                    $"\"phone_number\" ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns[RegexPatterns.Pattern.PhoneE164].PostgresPattern.ToString())}'"
                );
                
                // Check constraint for HomeAddress to allow only letters, digits, spaces, and hyphens
                t.HasCheckConstraint("CHK_User_HomeAddress_Valid",
                    $"\"home_address\" ~ '{SqlRegexHelper.SqlSafe(RegexPatterns.Patterns[RegexPatterns.Pattern.Address].PostgresPattern.ToString())}'");
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

            builder.Property(e => e.AvatarId)
                .HasColumnName("avatar_id");

            builder.Property(e => e.BirthDate)
                .HasColumnName("birth_date");

            builder.Property(e => e.Firstname)
                .HasMaxLength(RegexPatterns.Patterns[RegexPatterns.Pattern.Name].MaxLength)
                .HasColumnName("firstname");

            builder.Property(e => e.HomeAddress)
                .HasMaxLength(RegexPatterns.Patterns[RegexPatterns.Pattern.Address].MaxLength)
                .HasColumnName("home_address");

            builder.Property(e => e.Lastname)
                .HasMaxLength(RegexPatterns.Patterns[RegexPatterns.Pattern.Name].MaxLength)
                .HasColumnName("lastname");

            builder.Property(e => e.Login)
                .HasMaxLength(RegexPatterns.Patterns[RegexPatterns.Pattern.Email].MaxLength)
                .HasColumnName("login");

            builder.Property(e => e.PasswordHash)
                .HasMaxLength(256)
                .HasColumnName("password_hash");

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(RegexPatterns.Patterns[RegexPatterns.Pattern.PhoneE164].MaxLength)
                .HasColumnName("phone_number");

            builder.Property(e => e.RoleId)
                .HasColumnName("role_id");

            builder.Property(e => e.InstitutionId)
                .HasColumnName("institution_id")
                .IsRequired(false);

            // Relationships
            builder.HasOne(d => d.Avatar)
                .WithOne(p => p.User)
                .HasForeignKey<User>(d => d.AvatarId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("users_avatar_id_fkey");

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
