using LiteDB;

namespace xp_bank.Models;

public class Transaction
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string FromAccountId { get; set; } = string.Empty;

    public string ToMerchantId { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Status { get; set; } = "pending"; // pending, completed, blocked

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? BlockedReason { get; set; }
}

