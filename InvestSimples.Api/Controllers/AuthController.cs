using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using InvestSimples.Api.Data;
using InvestSimples.Api.DTOs;
using InvestSimples.Api.Models;
using InvestSimples.Api.Services;

namespace InvestSimples.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly InvestContext _context;
    private readonly JwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(InvestContext context, JwtService jwtService, IRefreshTokenService refreshTokenService)
    {
        _context = context;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Senha))
        {
            return BadRequest(new { error = "Email e senha são obrigatórios" });
        }

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Ativo);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
        {
            return Unauthorized(new { error = "Credenciais inválidas" });
        }

        usuario.UltimoLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var accessToken = _jwtService.GerarToken(usuario);
        var refreshToken = await _refreshTokenService.GerarRefreshTokenAsync(usuario.Id, GetClientIp());

        return Ok(new AuthResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiraEm = 900, // 15 min
            RefreshExpiraEm = 604800, // 7 dias
            Usuario = new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email
            }
        });
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrEmpty(request.Nome) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Senha))
        {
            return BadRequest(new { error = "Todos os campos são obrigatórios" });
        }

        if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
        {
            return Conflict(new { error = "Email já cadastrado" });
        }

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            SenhaHash = senhaHash,
            DataCriacao = DateTime.UtcNow,
            Ativo = true
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var accessToken = _jwtService.GerarToken(usuario);
        var refreshToken = await _refreshTokenService.GerarRefreshTokenAsync(usuario.Id, GetClientIp());

        return Ok(new AuthResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiraEm = 900,
            RefreshExpiraEm = 604800,
            Usuario = new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email
            }
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            return BadRequest(new { error = "Refresh token é obrigatório" });
        }

        var (valido, novoAccessToken, novoRefreshToken) = await _refreshTokenService.ValidarERenovarAsync(
            request.RefreshToken, request.AccessToken ?? string.Empty, GetClientIp());

        if (!valido || string.IsNullOrEmpty(novoAccessToken) || string.IsNullOrEmpty(novoRefreshToken))
        {
            return Unauthorized(new { error = "Refresh token inválido ou expirado" });
        }

        // Buscar usuário para retornar info
        var usuario = await _context.Usuarios
            .Where(u => u.Email == request.Email || u.Id == GetUsuarioIdFromToken(request.AccessToken))
            .Select(u => new UsuarioDto { Id = u.Id, Nome = u.Nome, Email = u.Email })
            .FirstOrDefaultAsync();

        return Ok(new AuthResponse
        {
            Token = novoAccessToken!,
            RefreshToken = novoRefreshToken!,
            ExpiraEm = 900,
            RefreshExpiraEm = 604800,
            Usuario = usuario!
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest request)
    {
        if (!string.IsNullOrEmpty(request.RefreshToken))
        {
            await _refreshTokenService.RevogarAsync(request.RefreshToken, GetClientIp());
        }
        return Ok(new { message = "Logout realizado com sucesso" });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> GetMe()
    {
        var usuarioId = GetUsuarioId();
        if (usuarioId == null) return Unauthorized();

        var usuario = await _context.Usuarios
            .Where(u => u.Id == usuarioId)
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email
            })
            .FirstOrDefaultAsync();

        if (usuario == null) return NotFound();

        return Ok(usuario);
    }

    [HttpPost("revoke-all")]
    [Authorize]
    public async Task<IActionResult> RevokeAll()
    {
        var usuarioId = GetUsuarioId();
        if (usuarioId == null) return Unauthorized();

        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UsuarioId == usuarioId.Value && rt.Ativo)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevogadoEm = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();

        return Ok(new { message = "Todos os tokens revogados" });
    }

    private int? GetUsuarioId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : null;
    }

    private int? GetUsuarioIdFromToken(string? token)
    {
        if (string.IsNullOrEmpty(token)) return null;
        try
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var claim = jwt.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : null;
        }
        catch
        {
            return null;
        }
    }

    private string GetClientIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}