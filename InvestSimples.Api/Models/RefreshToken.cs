using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestSimples.Api.Models;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string Token { get; set; } = string.Empty;

    [Required]
    public int UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario? Usuario { get; set; }

    public DateTime ExpiraEm { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? RevogadoEm { get; set; }
    public string? CriadoPorIp { get; set; }
    public string? RevogadoPorIp { get; set; }
    public string? SubstituidoPorToken { get; set; }
    public bool Revogado { get; set; } = false;

    public bool Ativo => !Revogado && ExpiraEm > DateTime.UtcNow;
}