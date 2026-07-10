using System.Security.Cryptography;
using InvestSimples.Api.Data;
using InvestSimples.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestSimples.Api.Services;

public interface IRefreshTokenService
{
    Task<string> GerarRefreshTokenAsync(int usuarioId, string clientIp);
    Task<(bool Valido, string? NovoAccessToken, string? NovoRefreshToken)> ValidarERenovarAsync(string refreshToken, string accessToken, string clientIp);
    Task RevogarAsync(string refreshToken, string clientIp);
    Task RevogarTodosAsync(int usuarioId);
}

public class RefreshTokenService : IRefreshTokenService
{
    private readonly InvestContext _context;
    private readonly JwtService _jwtService;
    private readonly ILogger<RefreshTokenService> _logger;

    public RefreshTokenService(InvestContext context, JwtService jwtService, ILogger<RefreshTokenService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<string> GerarRefreshTokenAsync(int usuarioId, string clientIp)
    {
        var token = GerarTokenSeguro();
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var refreshToken = new RefreshToken
        {
            Token = token,
            UsuarioId = usuarioId,
            ExpiraEm = DateTime.UtcNow.AddDays(7),
            CriadoEm = DateTime.UtcNow,
            CriadoPorIp = clientIp
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return token;
    }

    public async Task<(bool Valido, string? NovoAccessToken, string? NovoRefreshToken)> ValidarERenovarAsync(string refreshToken, string accessToken, string clientIp)
    {
        var tokenEntity = await _context.RefreshTokens
            .Include(rt => rt.Usuario)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (tokenEntity == null || !tokenEntity.Ativo)
        {
            _logger.LogWarning("Refresh token não encontrado ou inativo: {Token}", refreshToken[..10] + "...");
            return (false, null, null);
        }

        // Revogar token atual
        tokenEntity.Revogado = true;
        tokenEntity.RevogadoEm = DateTime.UtcNow;
        tokenEntity.RevogadoPorIp = clientIp;

        // Gerar novos tokens
        var novoAccessToken = _jwtService.GerarToken(tokenEntity.Usuario!);
        var novoRefreshToken = await GerarRefreshTokenAsync(tokenEntity.UsuarioId, clientIp);

        // Linkar tokens
        tokenEntity.SubstituidoPorToken = novoRefreshToken;

        await _context.SaveChangesAsync();

        return (true, novoAccessToken, novoRefreshToken);
    }

    public async Task RevogarAsync(string refreshToken, string clientIp)
    {
        var tokenEntity = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (tokenEntity != null && tokenEntity.Ativo)
        {
            tokenEntity.Revogado = true;
            tokenEntity.RevogadoEm = DateTime.UtcNow;
            tokenEntity.RevogadoPorIp = clientIp;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RevogarTodosAsync(int usuarioId)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UsuarioId == usuarioId && rt.Ativo)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.Revogado = true;
            token.RevogadoEm = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    private static string GerarTokenSeguro()
    {
        var bytes = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}