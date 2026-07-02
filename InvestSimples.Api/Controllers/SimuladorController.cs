using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InvestSimples.Api.Data;
using InvestSimples.Api.DTOs;
using InvestSimples.Api.Models;

namespace InvestSimples.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SimuladorController : ControllerBase
{
    private readonly InvestContext _context;

    public SimuladorController(InvestContext context)
    {
        _context = context;
    }

    [HttpPost]
    public ActionResult<SimulacaoResponse> Simular([FromBody] SimulacaoRequest request)
    {
        if (request.ValorInicial <= 0 || request.Anos <= 0)
        {
            return BadRequest(new { error = "Valor inicial e anos devem ser maiores que zero" });
        }

        var taxa = request.TaxaAnual ?? 10;
        var taxaMensal = (double)(taxa / 12 / 100);
        var meses = request.Anos * 12;
        var aplicacaoMensal = (double)request.ValorInicial;
        double saldo = 0;
        double totalInvestido = 0;

        for (int i = 0; i < meses; i++)
        {
            saldo += aplicacaoMensal;
            saldo += saldo * taxaMensal;
            totalInvestido += aplicacaoMensal;
        }

        var response = new SimulacaoResponse
        {
            Retorno = Math.Round((decimal)saldo, 2),
            TipoInvestimento = request.TipoInvestimento ?? "padrão",
            TaxaAnual = taxa,
            IdadeInicial = request.Idade,
            IdadeFinal = request.Idade + request.Anos,
            PeriodoAnos = request.Anos,
            TotalInvestido = Math.Round((decimal)totalInvestido, 2),
            AplicacaoMensal = request.ValorInicial
        };

        return Ok(response);
    }

    [HttpPost("salvar")]
    public async Task<ActionResult<SimulacaoSalvaResponse>> SalvarSimulacao([FromBody] SimulacaoRequest request)
    {
        var usuarioId = GetUsuarioId();
        if (usuarioId == null) return Unauthorized();

        var taxa = request.TaxaAnual ?? 10;
        var taxaMensal = (double)(taxa / 12 / 100);
        var meses = request.Anos * 12;
        var aplicacaoMensal = (double)request.ValorInicial;
        double saldo = 0;
        double totalInvestido = 0;

        for (int i = 0; i < meses; i++)
        {
            saldo += aplicacaoMensal;
            saldo += saldo * taxaMensal;
            totalInvestido += aplicacaoMensal;
        }

        var simulacao = new Simulacao
        {
            Nome = request.TipoInvestimento ?? "Simulação Personalizada",
            ValorInicial = request.ValorInicial,
            AporteMensal = request.ValorInicial,
            PrazoMeses = meses,
            TaxaAnual = taxa,
            ValorFinal = Math.Round((decimal)saldo, 2),
            TotalInvestido = Math.Round((decimal)totalInvestido, 2),
            RendimentoTotal = Math.Round((decimal)(saldo - totalInvestido), 2),
            UsuarioId = usuarioId.Value
        };

        _context.Simulacoes.Add(simulacao);
        await _context.SaveChangesAsync();

        return Ok(new SimulacaoSalvaResponse
        {
            Id = simulacao.Id,
            Nome = simulacao.Nome,
            ValorFinal = simulacao.ValorFinal,
            TotalInvestido = simulacao.TotalInvestido,
            RendimentoTotal = simulacao.RendimentoTotal,
            DataSimulacao = simulacao.DataSimulacao
        });
    }

    [HttpGet("historico")]
    public async Task<ActionResult<IEnumerable<SimulacaoSalvaResponse>>> GetHistorico()
    {
        var usuarioId = GetUsuarioId();
        if (usuarioId == null) return Unauthorized();

        var simulacoes = await _context.Simulacoes
            .Where(s => s.UsuarioId == usuarioId)
            .OrderByDescending(s => s.DataSimulacao)
            .Select(s => new SimulacaoSalvaResponse
            {
                Id = s.Id,
                Nome = s.Nome,
                ValorFinal = s.ValorFinal,
                TotalInvestido = s.TotalInvestido,
                RendimentoTotal = s.RendimentoTotal,
                DataSimulacao = s.DataSimulacao
            })
            .ToListAsync();

        return Ok(simulacoes);
    }

    private int? GetUsuarioId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : null;
    }
}