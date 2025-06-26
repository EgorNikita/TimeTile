using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations;

internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // Table Configuration
        builder.ToTable("refresh_tokens");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.Token })
            .HasDatabaseName("refresh_tokens_token_constraint")
            .AreNullsDistinct(false)
            .IsUnique();

        builder.HasIndex(e => new { e.UserId, e.RevokedAt })
            .HasDatabaseName("refresh_tokens_user_revoked_id");

        // Property Configuration
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(e => e.Token)
            .IsRequired()
            .HasMaxLength(512) // 88 chars for 64-byte Base64, plenty of head-room
            .HasColumnName("token");

        builder.Property(e => e.UserId)
            .HasColumnName("user_id");

        builder.Property(e => e.ExpiresAt)
            .HasColumnName("expires_at");

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at");
        
        builder.Property(e => e.CreatedByIp)
            .HasMaxLength(45) // IPv6 max length
            .HasColumnName("created_by_ip");

        builder.Property(e => e.RevokedAt)
            .HasColumnName("revoked_at");

        builder.Property(e => e.RevokedByIp)
            .HasMaxLength(45) // IPv6 max length
            .HasColumnName("revoked_by_ip");

        builder.Property(e => e.ReplaceTokenId)
            .HasColumnName("replace_token_id");

        // (1) User 1-n RefreshTokens
        builder.HasOne(e => e.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("refresh_tokens_user_id_fkey");

        // (2) Self-reference: replaced-by token
        builder.HasOne(e => e.ReplaceToken)
            .WithMany() // no back-collection
            .HasForeignKey(e => e.ReplaceTokenId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("refresh_tokens_replace_token_id_fkey");
    }
}