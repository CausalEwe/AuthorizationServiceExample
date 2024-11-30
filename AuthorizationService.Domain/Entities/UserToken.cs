namespace AuthorizationService.Domain.Entities;

public class UserToken
{
    public int UserId { get; set; }

    public string Token { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }
}