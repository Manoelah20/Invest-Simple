using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestSimples.Api.Data;
using InvestSimples.Api.DTOs;
using InvestSimples.Api.Models;

namespace InvestSimples.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CotacoesController : ControllerBase
{
    private readonly InvestContext _context;

    public CotacoesController(InvestContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CotacaoDto>>> GetCotacoes()
    {
        var cotacoes = await _context.Ativos
            .Where(a => a.IsAtivo && (a.Tipo == "Câmbio" || a.Tipo == "Cripto" || a.Tipo == "Índice"))
            .Select(a => new CotacaoDto
            {
                Nome = a.Codigo,
                Valor = a.PrecoAtual,
                Variacao = a.VariacaoDia
            })
            .ToListAsync();

        if (!cotacoes.Any())
        {
            cotacoes = GetMockCotacoes();
        }

        return Ok(cotacoes);
    }

    [HttpGet("todas")]
    public async Task<ActionResult<IEnumerable<CotacaoDto>>> GetTodasCotacoes()
    {
        var cotacoes = await _context.Ativos
            .Where(a => a.IsAtivo)
            .Select(a => new CotacaoDto
            {
                Nome = a.Codigo,
                Valor = a.PrecoAtual,
                Variacao = a.VariacaoDia
            })
            .ToListAsync();

        return Ok(cotacoes);
    }

    private static List<CotacaoDto> GetMockCotacoes()
    {
        var random = new Random();
        return new List<CotacaoDto>
        {
            new() { Nome = "USD/BRL", Valor = Math.Round(5.2m + (decimal)(random.NextDouble() * 0.4), 2), Variacao = Math.Round((decimal)(random.NextDouble() * 4 - 2), 2) },
            new() { Nome = "EUR/BRL", Valor = Math.Round(5.6m + (decimal)(random.NextDouble() * 0.4), 2), Variacao = Math.Round((decimal)(random.NextDouble() * 4 - 2), 2) },
            new() { Nome = "BTC/BRL", Valor = Math.Round(250000m + (decimal)(random.NextDouble() * 10000), 2), Variacao = Math.Round((decimal)(random.NextDouble() * 8 - 4), 2) },
            new() { Nome = "IBOVESPA", Valor = Math.Round(120000m + (decimal)(random.NextDouble() * 5000), 2), Variacao = Math.Round((decimal)(random.NextDouble() * 6 - 3), 2) },
            new() { Nome = "CDI", Valor = Math.Round(13.25m + (decimal)(random.NextDouble() * 0.5), 2), Variacao = Math.Round((decimal)(random.NextDouble() * 2 - 1), 2) },
            new() { Nome = "SELIC", Valor = Math.Round(10.5m + (decimal)(random.NextDouble() * 0.3), 2), Variacao = Math.Round((decimal)(random.NextDouble() * 1 - 0.5), 2) }
        };
    }
}