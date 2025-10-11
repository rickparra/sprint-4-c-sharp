using LiteDB;
using xp_bank.Data;
using xp_bank.Models;

namespace xp_bank.Services;

public class MerchantService
{
    private readonly LiteDbContext _context;
    private readonly ReceitaWsService _receitaWsService;
    private readonly ILogger<MerchantService> _logger;

    public MerchantService(
        LiteDbContext context, 
        ReceitaWsService receitaWsService,
        ILogger<MerchantService> logger)
    {
        _context = context;
        _receitaWsService = receitaWsService;
        _logger = logger;
    }

    public Merchant Create(Merchant merchant)
    {
        // Consulta a API ReceitaWS para validar o CNPJ
        var receitaInfo = _receitaWsService.ConsultarCnpjAsync(merchant.Cnpj).GetAwaiter().GetResult();

        if (receitaInfo != null)
        {
            // Verifica se é empresa de apostas
            if (_receitaWsService.IsEmpresaDeApostas(receitaInfo))
            {
                merchant.IsBlocked = true;
                _logger.LogWarning("Merchant {Name} bloqueado automaticamente - identificado como casa de apostas", merchant.Name);
            }

            // Atualiza informações com base na Receita
            if (!string.IsNullOrEmpty(receitaInfo.Nome))
            {
                merchant.Name = receitaInfo.Nome;
            }
        }

        _context.Merchants.Insert(merchant);
        return merchant;
    }

    public List<Merchant> Search(string? name = null, string? category = null, bool? isBlocked = null)
    {
        var query = _context.Merchants.FindAll().AsEnumerable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(m => m.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(m => m.Category == category);
        }

        if (isBlocked.HasValue)
        {
            query = query.Where(m => m.IsBlocked == isBlocked.Value);
        }

        return query
            .OrderByDescending(m => m.CreatedAt)
            .ToList();
    }

    public bool Update(string id, Merchant merchant)
    {
        // Se o CNPJ foi alterado, revalida com a ReceitaWS
        var existing = _context.Merchants.FindById(new ObjectId(id));
        if (existing != null && existing.Cnpj != merchant.Cnpj)
        {
            var receitaInfo = _receitaWsService.ConsultarCnpjAsync(merchant.Cnpj).GetAwaiter().GetResult();
            if (receitaInfo != null && _receitaWsService.IsEmpresaDeApostas(receitaInfo))
            {
                merchant.IsBlocked = true;
            }
        }

        return _context.Merchants.Update(merchant);
    }
}

