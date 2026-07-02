using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InvestSimples.Api.Data;
using InvestSimples.Api.DTOs;
using InvestSimples.Api.Models;

namespace InvestSimples.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarteiraController : ControllerBase
{
    private readonly InvestContext _context;

    public CarteiraController(InvestContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarteiraDto>>> GetCarteira()
    {
        var usuarioId = GetUsuarioId();
        if (usuarioId == null) return Unauthorized();

        var ativos = await _context.Ativos
            .Where(a => a.UsuarioId == usuarioId && a.IsAtivo && (a.Tipo == "Ações" || a.Tipo == "Renda Fixa" || a.Tipo == "Fundos"))
            .ToListAsync();

        if (!ativos.Any())
        {
            return Ok(GetMockCarteira());
        }

        var carteira = ativos.Select(a => new CarteiraDto
        {
            Ativo = a.Codigo,
            Tipo = a.Tipo,
            ValorInvestido = a.PrecoAtual * 100, // Simulação
            Rentabilidade = a.VariacaoDia,
            Risco = a.Tipo switch
            {
                "Ações" => "Alto",
                "Renda Fixa" => "Baixo",
                "Fundos" => "Médio",
                _ => "Médio"
            }
        }).ToList();

        return Ok(carteira);
    }

    [HttpGet("resumo")]
    public async Task<ActionResult<object>> GetResumoCarteira()
    {
        var usuarioId = GetUsuarioId();
        if (usuarioId == null) return Unauthorized();

        var ativos = await _context.Ativos
            .Where(a => a.UsuarioId == usuarioId && a.IsAtivo)
            .ToListAsync();

        var totalInvestido = ativos.Sum(a => a.PrecoAtual * 100);
        var totalRentabilidade = ativos.Average(a => a.VariacaoDia);

        return Ok(new
        {
            TotalInvestido = totalInvestido,
            RentabilidadeMedia = Math.Round(totalRentabilidade, 2),
            QuantidadeAtivos = ativos.Count,
            DistribuicaoPorTipo = ativos
                .GroupBy(a => a.Tipo)
                .Select(g => new { Tipo = g.Key, Quantidade = g.Count(), Valor = g.Sum(a => a.PrecoAtual * 100) })
        });
    }

    private int? GetUsuarioId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : null;
    }

    private static List<CarteiraDto> GetMockCarteira()
    {
        return new List<CarteiraDto>
        {
            new() { Ativo = "PETR4", Tipo = "Ações", ValorInvestido = 15000, Rentabilidade = 8.5m, Risco = "Alto" },
            new() { Ativo = "VALE3", Tipo = "Ações", ValorInvestido = 12000, Rentabilidade = 12.3m, Risco = "Alto" },
            new() { Ativo = "CDB Banco X", Tipo = "Renda Fixa", ValorInvestido = 25000, Rentabilidade = 13.2m, Risco = "Baixo" },
            new() { Ativo = "LCI Banco Y", Tipo = "Renda Fixa", ValorInvestido = 18000, Rentabilidade = 11.8m, Risco = "Baixo" },
            new() { Ativo = "FII HGLG11", Tipo = "Fundos", ValorInvestido = 8000, Rentabilidade = 9.7m, Risco = "Médio" }
        };
    }
}