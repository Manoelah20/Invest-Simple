using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestSimples.Api.Models;

public class Ativo
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Tipo { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecoAtual { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal VariacaoDia { get; set; }

    [MaxLength(50)]
    public string? Moeda { get; set; }

    public bool IsAtivo { get; set; } = true;

    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

    public int UsuarioId { get; set; }
    [ForeignKey(nameof(UsuarioId))]
    public Usuario? Usuario { get; set; }

    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}