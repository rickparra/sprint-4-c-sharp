using LiteDB;

namespace xp_bank.Models;

public class User
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

