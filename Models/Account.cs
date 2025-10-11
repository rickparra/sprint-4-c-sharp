using LiteDB;

namespace xp_bank.Models;

public class Account
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public decimal Balance { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

