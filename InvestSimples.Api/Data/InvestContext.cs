using Microsoft.EntityFrameworkCore;
using InvestSimples.Api.Models;

namespace InvestSimples.Api.Data;

public class InvestContext : DbContext
{
    public InvestContext(DbContextOptions<InvestContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Ativo> Ativos => Set<Ativo>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();
    public DbSet<Simulacao> Simulacoes => Set<Simulacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.SenhaHash).HasMaxLength(255);
        });

        modelBuilder.Entity<Ativo>(entity =>
        {
            entity.HasIndex(a => new { a.Codigo, a.UsuarioId }).IsUnique();
            entity.Property(a => a.PrecoAtual).HasPrecision(18, 2);
            entity.Property(a => a.VariacaoDia).HasPrecision(10, 2);
        });

        modelBuilder.Entity<Transacao>(entity =>
        {
            entity.Property(t => t.PrecoUnitario).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Simulacao>(entity =>
        {
            entity.Property(s => s.ValorInicial).HasPrecision(18, 2);
            entity.Property(s => s.AporteMensal).HasPrecision(18, 2);
            entity.Property(s => s.TaxaAnual).HasPrecision(5, 2);
            entity.Property(s => s.ValorFinal).HasPrecision(18, 2);
            entity.Property(s => s.TotalInvestido).HasPrecision(18, 2);
            entity.Property(s => s.RendimentoTotal).HasPrecision(18, 2);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var usuarioId = 1;
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("123456");

        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = usuarioId,
                Nome = "Usuário Teste",
                Email = "teste@investsimples.com",
                SenhaHash = senhaHash,
                DataCriacao = DateTime.UtcNow,
                Ativo = true
            }
        );

        modelBuilder.Entity<Ativo>().HasData(
            new Ativo { Id = 1, Codigo = "PETR4", Nome = "Petrobras PN", Tipo = "Ações", PrecoAtual = 35.50m, VariacaoDia = 2.15m, Moeda = "BRL", UsuarioId = usuarioId, DataAtualizacao = DateTime.UtcNow, IsAtivo = true },
            new Ativo { Id = 2, Codigo = "VALE3", Nome = "Vale ON", Tipo = "Ações", PrecoAtual = 68.90m, VariacaoDia = -1.20m, Moeda = "BRL", UsuarioId = usuarioId, DataAtualizacao = DateTime.UtcNow, IsAtivo = true },
            new Ativo { Id = 3, Codigo = "CDB001", Nome = "CDB Banco X 110% CDI", Tipo = "Renda Fixa", PrecoAtual = 1000.00m, VariacaoDia = 0.05m, Moeda = "BRL", UsuarioId = usuarioId, DataAtualizacao = DateTime.UtcNow, IsAtivo = true },
            new Ativo { Id = 4, Codigo = "LCI001", Nome = "LCI Banco Y 95% CDI", Tipo = "Renda Fixa", PrecoAtual = 1000.00m, VariacaoDia = 0.03m, Moeda = "BRL", UsuarioId = usuarioId, DataAtualizacao = DateTime.UtcNow, IsAtivo = true },
            new Ativo { Id = 5, Codigo = "HGLG11", Nome = "HGLG11 - FII Logística", Tipo = "Fundos", PrecoAtual = 185.40m, VariacaoDia = 1.80m, Moeda = "BRL", UsuarioId = usuarioId, DataAtualizacao = DateTime.UtcNow, IsAtivo = true },
            new Ativo { Id = 6, Codigo = "USD/BRL", Nome = "Dólar Americano", Tipo = "Câmbio", PrecoAtual = 5.25m, VariacaoDia = 0.45m, Moeda = "BRL", UsuarioId = usuarioId, DataAtualizacao = DateTime.UtcNow, IsAtivo = true },
            new Ativo { Id = 7, Codigo = "EUR/BRL", Nome = "Euro", Tipo = "Câmbio", PrecoAtual = 5.65m, VariacaoDia = -0.20m, Moeda = "BRL", UsuarioId = usuarioId, DataAtualizacao = DateTime.UtcNow, IsAtivo = true },
            new Ativo { Id = 8, Codigo = "BTC/BRL", Nome = "Bitcoin", Tipo = "Cripto", PrecoAtual = 265000.00m, VariacaoDia = 3.50m, Moeda = "BRL", UsuarioId = usuarioId, DataAtualizacao = DateTime.UtcNow, IsAtivo = true },
            new Ativo { Id = 9, Codigo = "IBOV", Nome = "IBOVESPA", Tipo = "Índice", PrecoAtual = 125000.00m, VariacaoDia = 1.20m, Moeda = "PONTOS", UsuarioId = usuarioId, DataAtualizacao = DateTime.UtcNow, IsAtivo = true }
        );
    }
}