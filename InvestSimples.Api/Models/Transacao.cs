using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestSimples.Api.Models;

public class Transacao
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string Tipo { get; set; } = string.Empty; // "COMPRA" ou "VENDA"

    [Required]
    public int Quantidade { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecoUnitario { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorTotal => Quantidade * PrecoUnitario;

    public DateTime DataTransacao { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Observacao { get; set; }

    public int AtivoId { get; set; }
    [ForeignKey(nameof(AtivoId))]
    public Ativo? Ativo { get; set; }

    public int UsuarioId { get; set; }
    [ForeignKey(nameof(UsuarioId))]
    public Usuario? Usuario { get; set; }
}