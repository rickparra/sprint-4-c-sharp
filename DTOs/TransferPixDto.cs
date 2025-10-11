namespace xp_bank.DTOs;

public class TransferPixDto
{
    public string FromAccountId { get; set; } = string.Empty;
    public string ToMerchantId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

