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

    public AuthController(InvestContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
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

        var token = _jwtService.GerarToken(usuario);

        return Ok(new AuthResponse
        {
            Token = token,
            ExpiraEm = 3600,
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

        var token = _jwtService.GerarToken(usuario);

        return Ok(new AuthResponse
        {
            Token = token,
            ExpiraEm = 3600,
            Usuario = new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email
            }
        });
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

    private int? GetUsuarioId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : null;
    }
}