namespace xp_bank.DTOs;

public class CreateAccountDto
{
    public string UserId { get; set; } = string.Empty;
    public decimal InitialBalance { get; set; } = 0;
    public string AccountNumber { get; set; } = string.Empty;
}

