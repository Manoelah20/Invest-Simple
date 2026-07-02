using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestSimples.Api.Models;

public class Usuario
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string SenhaHash { get; set; } = string.Empty;

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public DateTime? UltimoLogin { get; set; }

    public bool Ativo { get; set; } = true;

    [NotMapped]
    public string? Token { get; set; }

    public ICollection<Ativo> Ativos { get; set; } = new List<Ativo>();
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    public ICollection<Simulacao> Simulacoes { get; set; } = new List<Simulacao>();
}