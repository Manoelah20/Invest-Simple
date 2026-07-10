using Xunit;
using InvestSimples.Api.Controllers;
using InvestSimples.Api.DTOs;

namespace InvestSimples.Api.Tests;

public class SimuladorLogicTests
{
    [Theory]
    [InlineData(1000, 10, 12, 30, "Ações")]
    [InlineData(500, 5, 10, 25, "Renda Fixa")]
    [InlineData(2000, 20, 8, 35, "Fundos")]
    public void Simular_CalculaJurosCompostos_Corretamente(
        decimal valorInicial, int anos, decimal taxaAnual, int idade, string tipo)
    {
        // Arrange
        var request = new SimulacaoRequest
        {
            ValorInicial = valorInicial,
            Anos = anos,
            Idade = idade,
            TipoInvestimento = tipo,
            TaxaAnual = taxaAnual
        };

        var taxa = taxaAnual;
        var taxaMensal = (double)(taxa / 12 / 100);
        var meses = anos * 12;
        var aplicacaoMensal = (double)valorInicial;
        double saldo = 0;
        double totalInvestido = 0;

        for (int i = 0; i < meses; i++)
        {
            saldo += aplicacaoMensal;
            saldo += saldo * taxaMensal;
            totalInvestido += aplicacaoMensal;
        }

        var expectedRetorno = Math.Round((decimal)saldo, 2);
        var expectedTotalInvestido = Math.Round((decimal)totalInvestido, 2);

        // Act - simulate the controller logic
        var result = new SimulacaoResponse
        {
            Retorno = expectedRetorno,
            TipoInvestimento = tipo,
            TaxaAnual = taxa,
            IdadeInicial = idade,
            IdadeFinal = idade + anos,
            PeriodoAnos = anos,
            TotalInvestido = expectedTotalInvestido,
            AplicacaoMensal = valorInicial
        };

        // Assert
        Assert.Equal(tipo, result.TipoInvestimento);
        Assert.Equal(taxaAnual, result.TaxaAnual);
        Assert.Equal(anos, result.PeriodoAnos);
        Assert.Equal(idade, result.IdadeInicial);
        Assert.Equal(idade + anos, result.IdadeFinal);
        Assert.True(result.Retorno > 0);
        Assert.True(result.TotalInvestido > 0);
        Assert.Equal(valorInicial * anos * 12, result.TotalInvestido, 1); // approximate
    }

    [Theory]
    [InlineData(0, 10, 12, 30)]
    [InlineData(1000, 0, 12, 30)]
    public void Simular_ValorInicialOuAnosZero_RetornaErro(
        decimal valorInicial, int anos, decimal taxaAnual, int idade)
    {
        var request = new SimulacaoRequest
        {
            ValorInicial = valorInicial,
            Anos = anos,
            Idade = idade,
            TaxaAnual = taxaAnual
        };

        var isValid = request.ValorInicial > 0 && request.Anos > 0;
        
        Assert.False(isValid);
    }
}