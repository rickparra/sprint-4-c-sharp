namespace xp_bank.DTOs;

public class UpdateMerchantDto
{
    public string? Name { get; set; }
    public string? Cnpj { get; set; }
    public string? Category { get; set; }
    public bool? IsBlocked { get; set; }
}

