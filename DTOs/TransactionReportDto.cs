namespace xp_bank.DTOs;

public class TransactionReportDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}

