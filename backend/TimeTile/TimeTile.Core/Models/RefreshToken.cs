namespace TimeTile.Core.Models;

public class RefreshToken
{
    public int Id { get; set; }
    
    public required string Token { get; set; }
    
    public required int UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedByIp { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    
    public int? ReplaceTokenId { get; set; }
    public virtual RefreshToken? ReplaceToken { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;
}