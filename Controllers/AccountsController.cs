using Microsoft.AspNetCore.Mvc;
using LiteDB;
using xp_bank.Data;
using xp_bank.DTOs;
using xp_bank.Models;

namespace xp_bank.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly LiteDbContext _context;

    public AccountsController(LiteDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<List<Account>> GetAll()
    {
        var accounts = _context.Accounts.FindAll().ToList();
        return Ok(accounts);
    }

    [HttpGet("{id}")]
    public ActionResult<Account> GetById(string id)
    {
        var account = _context.Accounts.FindById(new ObjectId(id));
        
        if (account == null)
            return NotFound(new { message = "Conta não encontrada" });

        return Ok(account);
    }

    [HttpGet("user/{userId}")]
    public ActionResult<List<Account>> GetByUserId(string userId)
    {
        var accounts = _context.Accounts
            .Find(a => a.UserId == userId)
            .ToList();

        return Ok(accounts);
    }

    [HttpPost]
    public ActionResult<Account> Create([FromBody] CreateAccountDto dto)
    {
        // Verifica se o usuário existe
        var user = _context.Users.FindById(new ObjectId(dto.UserId));
        if (user == null)
            return BadRequest(new { message = "Usuário não encontrado" });

        var account = new Account
        {
            Id = ObjectId.NewObjectId(),
            UserId = dto.UserId,
            Balance = dto.InitialBalance,
            AccountNumber = dto.AccountNumber,
            CreatedAt = DateTime.UtcNow
        };

        _context.Accounts.Insert(account);
        return CreatedAtAction(nameof(GetById), new { id = account.Id.ToString() }, account);
    }

    [HttpPut("{id}")]
    public ActionResult Update(string id, [FromBody] UpdateAccountDto dto)
    {
        var account = _context.Accounts.FindById(new ObjectId(id));
        
        if (account == null)
            return NotFound(new { message = "Conta não encontrada" });

        if (dto.Balance.HasValue)
            account.Balance = dto.Balance.Value;

        if (!string.IsNullOrEmpty(dto.AccountNumber))
            account.AccountNumber = dto.AccountNumber;

        _context.Accounts.Update(account);
        return Ok(account);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(string id)
    {
        var deleted = _context.Accounts.Delete(new ObjectId(id));
        
        if (!deleted)
            return NotFound(new { message = "Conta não encontrada" });

        return NoContent();
    }
}

