using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InvestSimples.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    SenhaHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UltimoLogin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ativos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    PrecoAtual = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VariacaoDia = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Moeda = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsAtivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ativos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ativos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Simulacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ValorInicial = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AporteMensal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrazoMeses = table.Column<int>(type: "INTEGER", nullable: false),
                    TaxaAnual = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ValorFinal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalInvestido = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RendimentoTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DataSimulacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Simulacoes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DataTransacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AtivoId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transacoes_Ativos_AtivoId",
                        column: x => x.AtivoId,
                        principalTable: "Ativos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transacoes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Ativo", "DataCriacao", "Email", "Nome", "SenhaHash", "UltimoLogin" },
                values: new object[] { 1, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "teste@investsimples.com", "Usuário Teste", "$2a$11$4dPSHSI0FAH8fLAsMk4hLesKK.6q.QVLujD7FyJ35N/jAOVggfIc2", null });

            migrationBuilder.InsertData(
                table: "Ativos",
                columns: new[] { "Id", "Codigo", "DataAtualizacao", "IsAtivo", "Moeda", "Nome", "PrecoAtual", "Tipo", "UsuarioId", "VariacaoDia" },
                values: new object[,]
                {
                    { 1, "PETR4", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "BRL", "Petrobras PN", 35.50m, "Ações", 1, 2.15m },
                    { 2, "VALE3", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "BRL", "Vale ON", 68.90m, "Ações", 1, -1.20m },
                    { 3, "CDB001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "BRL", "CDB Banco X 110% CDI", 1000.00m, "Renda Fixa", 1, 0.05m },
                    { 4, "LCI001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "BRL", "LCI Banco Y 95% CDI", 1000.00m, "Renda Fixa", 1, 0.03m },
                    { 5, "HGLG11", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "BRL", "HGLG11 - FII Logística", 185.40m, "Fundos", 1, 1.80m },
                    { 6, "USD/BRL", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "BRL", "Dólar Americano", 5.25m, "Câmbio", 1, 0.45m },
                    { 7, "EUR/BRL", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "BRL", "Euro", 5.65m, "Câmbio", 1, -0.20m },
                    { 8, "BTC/BRL", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "BRL", "Bitcoin", 265000.00m, "Cripto", 1, 3.50m },
                    { 9, "IBOV", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "PONTOS", "IBOVESPA", 125000.00m, "Índice", 1, 1.20m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ativos_Codigo_UsuarioId",
                table: "Ativos",
                columns: new[] { "Codigo", "UsuarioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ativos_UsuarioId",
                table: "Ativos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Simulacoes_UsuarioId",
                table: "Simulacoes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_AtivoId",
                table: "Transacoes",
                column: "AtivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_UsuarioId",
                table: "Transacoes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Simulacoes");

            migrationBuilder.DropTable(
                name: "Transacoes");

            migrationBuilder.DropTable(
                name: "Ativos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
