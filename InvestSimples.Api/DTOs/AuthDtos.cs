namespace InvestSimples.Api.DTOs;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Tipo { get; set; } = "Bearer";
    public int ExpiraEm { get; set; } = 3600;
    public UsuarioDto Usuario { get; set; } = new();
}

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class CotacaoDto
{
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal Variacao { get; set; }
}

public class CarteiraDto
{
    public string Ativo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal ValorInvestido { get; set; }
    public decimal Rentabilidade { get; set; }
    public string Risco { get; set; } = string.Empty;
}

public class SimulacaoRequest
{
    public decimal ValorInicial { get; set; }
    public int Anos { get; set; }
    public int Idade { get; set; }
    public string? TipoInvestimento { get; set; }
    public decimal? TaxaAnual { get; set; }
}

public class SimulacaoResponse
{
    public decimal Retorno { get; set; }
    public string TipoInvestimento { get; set; } = string.Empty;
    public decimal TaxaAnual { get; set; }
    public int IdadeInicial { get; set; }
    public int IdadeFinal { get; set; }
    public int PeriodoAnos { get; set; }
    public decimal TotalInvestido { get; set; }
    public decimal AplicacaoMensal { get; set; }
}

public class SimulacaoSalvaResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal ValorFinal { get; set; }
    public decimal TotalInvestido { get; set; }
    public decimal RendimentoTotal { get; set; }
    public DateTime DataSimulacao { get; set; }
}