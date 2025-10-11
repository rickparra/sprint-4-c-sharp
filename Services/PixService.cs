using LiteDB;
using xp_bank.Data;
using xp_bank.DTOs;
using xp_bank.Models;

namespace xp_bank.Services;

public class PixService
{
    private readonly LiteDbContext _context;
    private readonly ILogger<PixService> _logger;

    public PixService(LiteDbContext context, ILogger<PixService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public (bool Success, string Message, Transaction? Transaction) Transfer(TransferPixDto dto)
    {
        // 1. Validar conta origem
        var accountId = new ObjectId(dto.FromAccountId);
        var account = _context.Accounts.FindById(accountId);
        if (account == null)
        {
            return (false, "Conta de origem não encontrada", null);
        }

        // 2. Validar saldo
        if (account.Balance < dto.Amount)
        {
            return (false, "Saldo insuficiente", null);
        }

        // 3. Validar merchant destino
        var merchantId = new ObjectId(dto.ToMerchantId);
        var merchant = _context.Merchants.FindById(merchantId);
        if (merchant == null)
        {
            return (false, "Merchant não encontrado", null);
        }

        // 4. Verificar se merchant está bloqueado
        if (merchant.IsBlocked)
        {
            var transaction = new Transaction
            {
                Id = ObjectId.NewObjectId(),
                FromAccountId = dto.FromAccountId,
                ToMerchantId = dto.ToMerchantId,
                Amount = dto.Amount,
                Status = "blocked",
                BlockedReason = $"Transferência bloqueada: {merchant.Name} é uma casa de apostas",
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Insert(transaction);

            _logger.LogWarning(
                "Transferência bloqueada para merchant {MerchantName} (casa de apostas)", 
                merchant.Name
            );

            return (false, transaction.BlockedReason!, transaction);
        }

        // 5. Realizar transferência
        account.Balance -= dto.Amount;
        _context.Accounts.Update(account);

        var successTransaction = new Transaction
        {
            Id = ObjectId.NewObjectId(),
            FromAccountId = dto.FromAccountId,
            ToMerchantId = dto.ToMerchantId,
            Amount = dto.Amount,
            Status = "completed",
            CreatedAt = DateTime.UtcNow
        };

        _context.Transactions.Insert(successTransaction);

        _logger.LogInformation(
            "Transferência de R$ {Amount} realizada com sucesso para {MerchantName}",
            dto.Amount,
            merchant.Name
        );

        return (true, "Transferência realizada com sucesso", successTransaction);
    }

    public List<Transaction> GetAllTransactions()
    {
        return _context.Transactions
            .FindAll()
            .OrderByDescending(t => t.CreatedAt)
            .ToList();
    }

    public List<TransactionReportDto> GetReport()
    {
        var report = _context.Transactions
            .FindAll()
            .GroupBy(t => t.Status)
            .Select(g => new TransactionReportDto
            {
                Status = g.Key,
                Count = g.Count(),
                TotalAmount = g.Sum(t => t.Amount)
            })
            .ToList();

        return report;
    }
}

