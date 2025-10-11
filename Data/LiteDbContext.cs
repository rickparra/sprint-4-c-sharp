using LiteDB;
using xp_bank.Models;

namespace xp_bank.Data;

public class LiteDbContext : IDisposable
{
    private readonly LiteDatabase _database;

    public LiteDbContext(IConfiguration configuration)
    {
        var connectionString = configuration["LiteDB:ConnectionString"] 
            ?? "Filename=SafePix.db;Connection=shared";
        _database = new LiteDatabase(connectionString);
    }

    public ILiteCollection<User> Users => _database.GetCollection<User>("Users");
    public ILiteCollection<Account> Accounts => _database.GetCollection<Account>("Accounts");
    public ILiteCollection<Merchant> Merchants => _database.GetCollection<Merchant>("Merchants");
    public ILiteCollection<Transaction> Transactions => _database.GetCollection<Transaction>("Transactions");

    public void Dispose()
    {
        _database?.Dispose();
    }
}

