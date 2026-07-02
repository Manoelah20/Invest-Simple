using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestSimples.Api.Models;

public class Simulacao
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorInicial { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal AporteMensal { get; set; }

    [Required]
    [Range(1, 100)]
    public int PrazoMeses { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxaAnual { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorFinal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalInvestido { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RendimentoTotal { get; set; }

    public DateTime DataSimulacao { get; set; } = DateTime.UtcNow;

    public int UsuarioId { get; set; }
    [ForeignKey(nameof(UsuarioId))]
    public Usuario? Usuario { get; set; }
}