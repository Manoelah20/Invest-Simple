using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Xunit;
using InvestSimples.Api.Data;
using InvestSimples.Api.Models;
using InvestSimples.Api.DTOs;

namespace InvestSimples.Api.Tests;

public class AuthLogicTests
{
    private readonly InvestContext _context;

    public AuthLogicTests()
    {
        var options = new DbContextOptionsBuilder<InvestContext>()
            .UseInMemoryDatabase("AuthTestDb")
            .Options;

        _context = new InvestContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public void HashPassword_MesmaSenha_GeraHashDiferenteCadaVez()
    {
        var senha = "123456";
        var hash1 = BCrypt.Net.BCrypt.HashPassword(senha);
        var hash2 = BCrypt.Net.BCrypt.HashPassword(senha);

        Assert.NotEqual(hash1, hash2); // BCrypt uses salt
        Assert.True(BCrypt.Net.BCrypt.Verify(senha, hash1));
        Assert.True(BCrypt.Net.BCrypt.Verify(senha, hash2));
    }

    [Fact]
    public void VerifyPassword_SenhaCorreta_RetornaTrue()
    {
        var senha = "minhasenha123";
        var hash = BCrypt.Net.BCrypt.HashPassword(senha);
        
        var result = BCrypt.Net.BCrypt.Verify(senha, hash);
        
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_SenhaIncorreta_RetornaFalse()
    {
        var senha = "minhasenha123";
        var hash = BCrypt.Net.BCrypt.HashPassword(senha);
        
        var result = BCrypt.Net.BCrypt.Verify("senhaerrada", hash);
        
        Assert.False(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}