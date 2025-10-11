using Microsoft.AspNetCore.Mvc;
using LiteDB;
using xp_bank.Data;
using xp_bank.DTOs;
using xp_bank.Models;

namespace xp_bank.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly LiteDbContext _context;

    public UsersController(LiteDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<List<User>> GetAll()
    {
        var users = _context.Users.FindAll().ToList();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public ActionResult<User> GetById(string id)
    {
        var user = _context.Users.FindById(new ObjectId(id));
        
        if (user == null)
            return NotFound(new { message = "Usuário não encontrado" });

        return Ok(user);
    }

    [HttpPost]
    public ActionResult<User> Create([FromBody] CreateUserDto dto)
    {
        var user = new User
        {
            Id = ObjectId.NewObjectId(),
            Name = dto.Name,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Insert(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id.ToString() }, user);
    }

    [HttpPut("{id}")]
    public ActionResult Update(string id, [FromBody] UpdateUserDto dto)
    {
        var user = _context.Users.FindById(new ObjectId(id));
        
        if (user == null)
            return NotFound(new { message = "Usuário não encontrado" });

        if (!string.IsNullOrEmpty(dto.Name))
            user.Name = dto.Name;

        if (!string.IsNullOrEmpty(dto.Email))
            user.Email = dto.Email;

        _context.Users.Update(user);
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(string id)
    {
        var deleted = _context.Users.Delete(new ObjectId(id));
        
        if (!deleted)
            return NotFound(new { message = "Usuário não encontrado" });

        return NoContent();
    }
}

