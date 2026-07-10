using System.Net.Http.Json;
using System.Text.Json;
using InvestSimples.Api.Configuration;
using Microsoft.Extensions.Options;

namespace InvestSimples.Api.Services;

public interface IBrapiService
{
    Task<List<CotacaoBrapi>> GetCotacoesAsync(CancellationToken ct = default);
}

public class BrapiService : IBrapiService
{
    private readonly HttpClient _httpClient;
    private readonly BrapiOptions _options;

    private static readonly string[] Symbols = new[]
    {
        "PETR4.SA", "VALE3.SA", "ITUB4.SA", "BBDC4.SA",
        "ABEV3.SA", "WEGE3.SA", "RENT3.SA", "BBAS3.SA",
        "BTC-USD", "ETH-USD", "USDBRL=X", "EURBRL=X"
    };

    public BrapiService(HttpClient httpClient, IOptions<BrapiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<List<CotacaoBrapi>> GetCotacoesAsync(CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            return GetMockCotacoes();
        }

        var symbols = string.Join(",", Symbols);
        var url = $"{_options.BaseUrl}/quote/{symbols}?token={_options.ApiKey}";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<BrapiResponse>(url, ct);

            if (response?.Results == null)
            {
                return GetMockCotacoes();
            }

            return response.Results
                .Select(r => new CotacaoBrapi(
                    r.Symbol ?? string.Empty,
                    r.LongName ?? r.ShortName ?? r.Symbol ?? string.Empty,
                    r.RegularMarketPrice ?? 0,
                    r.RegularMarketChangePercent ?? 0,
                    r.Currency ?? "BRL",
                    DeterminarTipo(r.Symbol ?? string.Empty)
                ))
                .ToList();
        }
        catch (Exception)
        {
            return GetMockCotacoes();
        }
    }

    private static string DeterminarTipo(string simbolo)
    {
        if (simbolo.EndsWith(".SA")) return "Ações";
        if (simbolo.StartsWith("BTC") || simbolo.StartsWith("ETH")) return "Cripto";
        if (simbolo.Contains("USD") || simbolo.Contains("EUR")) return "Câmbio";
        if (simbolo.Contains("11")) return "FIIs";
        return "Outros";
    }

    private static List<CotacaoBrapi> GetMockCotacoes() => new()
    {
        new CotacaoBrapi("USD/BRL", "Dólar Americano", 5.25m, 0.45m, "BRL", "Câmbio"),
        new CotacaoBrapi("EUR/BRL", "Euro", 5.65m, -0.20m, "BRL", "Câmbio"),
        new CotacaoBrapi("BTC/BRL", "Bitcoin", 265000m, 3.5m, "BRL", "Cripto"),
        new CotacaoBrapi("IBOV", "IBOVESPA", 125000m, 1.2m, "PONTOS", "Índice"),
        new CotacaoBrapi("PETR4.SA", "Petrobras PN", 35.50m, 2.15m, "BRL", "Ações"),
        new CotacaoBrapi("VALE3.SA", "Vale ON", 68.90m, -1.20m, "BRL", "Ações")
    };
}

public record CotacaoBrapi(
    string Simbolo,
    string Nome,
    decimal Preco,
    decimal Variacao,
    string Moeda,
    string Tipo
);

internal record BrapiResponse(List<BrapiResult>? Results);

internal record BrapiResult(
    string? Symbol,
    string? ShortName,
    string? LongName,
    decimal? RegularMarketPrice,
    decimal? RegularMarketChangePercent,
    string? Currency
);