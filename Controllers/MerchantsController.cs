using Microsoft.AspNetCore.Mvc;
using LiteDB;
using xp_bank.Data;
using xp_bank.DTOs;
using xp_bank.Models;
using xp_bank.Services;

namespace xp_bank.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MerchantsController : ControllerBase
{
    private readonly LiteDbContext _context;
    private readonly MerchantService _merchantService;

    public MerchantsController(LiteDbContext context, MerchantService merchantService)
    {
        _context = context;
        _merchantService = merchantService;
    }

    [HttpGet]
    public ActionResult<List<Merchant>> GetAll()
    {
        var merchants = _context.Merchants.FindAll().ToList();
        return Ok(merchants);
    }

    [HttpGet("{id}")]
    public ActionResult<Merchant> GetById(string id)
    {
        var merchant = _context.Merchants.FindById(new ObjectId(id));
        
        if (merchant == null)
            return NotFound(new { message = "Merchant não encontrado" });

        return Ok(merchant);
    }

    [HttpGet("search")]
    public ActionResult<List<Merchant>> Search(
        [FromQuery] string? name = null,
        [FromQuery] string? category = null,
        [FromQuery] bool? isBlocked = null)
    {
        var merchants = _merchantService.Search(name, category, isBlocked);
        return Ok(merchants);
    }

    [HttpPost]
    public ActionResult<Merchant> Create([FromBody] CreateMerchantDto dto)
    {
        var merchant = new Merchant
        {
            Id = ObjectId.NewObjectId(),
            Name = dto.Name,
            Cnpj = dto.Cnpj,
            Category = dto.Category,
            CreatedAt = DateTime.UtcNow
        };

        var createdMerchant = _merchantService.Create(merchant);
        return CreatedAtAction(nameof(GetById), new { id = createdMerchant.Id.ToString() }, createdMerchant);
    }

    [HttpPut("{id}")]
    public ActionResult Update(string id, [FromBody] UpdateMerchantDto dto)
    {
        var merchant = _context.Merchants.FindById(new ObjectId(id));
        
        if (merchant == null)
            return NotFound(new { message = "Merchant não encontrado" });

        if (!string.IsNullOrEmpty(dto.Name))
            merchant.Name = dto.Name;

        if (!string.IsNullOrEmpty(dto.Cnpj))
            merchant.Cnpj = dto.Cnpj;

        if (!string.IsNullOrEmpty(dto.Category))
            merchant.Category = dto.Category;

        if (dto.IsBlocked.HasValue)
            merchant.IsBlocked = dto.IsBlocked.Value;

        var updated = _merchantService.Update(id, merchant);
        
        if (!updated)
            return NotFound(new { message = "Merchant não encontrado" });

        return Ok(merchant);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(string id)
    {
        var deleted = _context.Merchants.Delete(new ObjectId(id));
        
        if (!deleted)
            return NotFound(new { message = "Merchant não encontrado" });

        return NoContent();
    }
}

