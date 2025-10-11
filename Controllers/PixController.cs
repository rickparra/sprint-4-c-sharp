using Microsoft.AspNetCore.Mvc;
using xp_bank.DTOs;
using xp_bank.Services;

namespace xp_bank.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PixController : ControllerBase
{
    private readonly PixService _pixService;

    public PixController(PixService pixService)
    {
        _pixService = pixService;
    }

    [HttpPost("transfer")]
    public ActionResult Transfer([FromBody] TransferPixDto dto)
    {
        var (success, message, transaction) = _pixService.Transfer(dto);

        if (!success)
        {
            return BadRequest(new 
            { 
                message = message,
                transaction = transaction 
            });
        }

        return Ok(new 
        { 
            message = message,
            transaction = transaction 
        });
    }

    [HttpGet("transactions")]
    public ActionResult GetTransactions()
    {
        var transactions = _pixService.GetAllTransactions();
        return Ok(transactions);
    }

    [HttpGet("report")]
    public ActionResult GetReport()
    {
        var report = _pixService.GetReport();
        return Ok(report);
    }
}

