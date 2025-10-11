using LiteDB;

namespace xp_bank.Models;

public class Merchant
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public bool IsBlocked { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

