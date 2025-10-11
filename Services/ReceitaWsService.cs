using System.Text.Json.Serialization;

namespace xp_bank.Services;

public class ReceitaWsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReceitaWsService> _logger;

    public ReceitaWsService(HttpClient httpClient, ILogger<ReceitaWsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ReceitaWsResponse?> ConsultarCnpjAsync(string cnpj)
    {
        try
        {
            // Remove caracteres especiais do CNPJ
            cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");

            var response = await _httpClient.GetFromJsonAsync<ReceitaWsResponse>(
                $"https://www.receitaws.com.br/v1/cnpj/{cnpj}"
            );

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar CNPJ {Cnpj}", cnpj);
            return null;
        }
    }

    public bool IsEmpresaDeApostas(ReceitaWsResponse? response)
    {
        if (response == null)
            return false;

        // Verifica se a atividade principal contém termos relacionados a apostas
        var termosApostas = new[] { "aposta", "jogo", "loteria", "cassino", "bet", "gaming" };

        // Verifica na atividade principal
        if (response.AtividadePrincipal != null)
        {
            foreach (var atividade in response.AtividadePrincipal)
            {
                if (termosApostas.Any(termo => 
                    atividade.Text?.Contains(termo, StringComparison.OrdinalIgnoreCase) == true))
                {
                    return true;
                }
            }
        }

        // Verifica no nome fantasia
        if (!string.IsNullOrEmpty(response.Fantasia) && 
            termosApostas.Any(termo => response.Fantasia.Contains(termo, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        // Verifica na razão social
        if (!string.IsNullOrEmpty(response.Nome) && 
            termosApostas.Any(termo => response.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return false;
    }
}

public class ReceitaWsResponse
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("nome")]
    public string? Nome { get; set; }

    [JsonPropertyName("fantasia")]
    public string? Fantasia { get; set; }

    [JsonPropertyName("cnpj")]
    public string? Cnpj { get; set; }

    [JsonPropertyName("atividade_principal")]
    public List<AtividadeEconomica>? AtividadePrincipal { get; set; }

    [JsonPropertyName("atividades_secundarias")]
    public List<AtividadeEconomica>? AtividadesSecundarias { get; set; }
}

public class AtividadeEconomica
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

