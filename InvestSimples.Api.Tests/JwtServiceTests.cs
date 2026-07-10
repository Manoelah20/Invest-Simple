using System.Security.Claims;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Xunit;
using InvestSimples.Api.Data;
using InvestSimples.Api.Models;
using InvestSimples.Api.Services;
using InvestSimples.Api.DTOs;

namespace InvestSimples.Api.Tests;

public class JwtServiceTests
{
    private readonly JwtService _jwtService;
    private readonly InvestContext _context;

    public JwtServiceTests()
    {
        var options = new DbContextOptionsBuilder<InvestContext>()
            .UseInMemoryDatabase("JwtTestDb")
            .Options;

        _context = new InvestContext(options);
        _context.Database.EnsureCreated();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["JwtSettings:SecretKey"] = "InvestSimplesSecretKey2024!@#SuperSecretKey123456789",
                ["JwtSettings:Issuer"] = "InvestSimplesAPI",
                ["JwtSettings:Audience"] = "InvestSimplesClient",
                ["JwtSettings:ExpiryMinutes"] = "60"
            }!)
            .Build();

        _jwtService = new JwtService(config);
    }

    [Fact]
    public void GerarToken_UsuarioValido_RetornaTokenJwt()
    {
        var user = new Usuario
        {
            Id = 1,
            Nome = "Test User",
            Email = "test@test.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            DataCriacao = DateTime.UtcNow,
            Ativo = true
        };

        var token = _jwtService.GerarToken(user);

        Assert.NotNull(token);
        Assert.NotEmpty(token);
        
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        Assert.Equal("InvestSimplesAPI", jwtToken.Issuer);
        Assert.Equal("InvestSimplesClient", jwtToken.Audiences.First());
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == "1");
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Email && c.Value == "test@test.com");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}