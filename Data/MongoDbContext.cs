using MongoDB.Driver;
using xp_bank.Models;

namespace xp_bank.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionString"];
        var databaseName = configuration["MongoDB:DatabaseName"];

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    public IMongoCollection<Account> Accounts => _database.GetCollection<Account>("Accounts");
    public IMongoCollection<Merchant> Merchants => _database.GetCollection<Merchant>("Merchants");
    public IMongoCollection<Transaction> Transactions => _database.GetCollection<Transaction>("Transactions");
}

