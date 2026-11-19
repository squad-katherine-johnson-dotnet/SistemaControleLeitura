using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Services;
using SistemaControleLeitura.Infrastructure.Data;
using SistemaControleLeitura.Infrastructure.Repositories;

namespace SistemaControleLeitura.Tests.Application
{
    public class MetaAnualServiceTests
    {
        private static (AppDbContext Context, SqliteConnection Connection) CriarContextoSqliteEmMemoria()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            // Desativa a verificação de FK na conexão isolada de teste para não depender da entidade Usuario
            using (var pragmaCmd = connection.CreateCommand())
            {
                pragmaCmd.CommandText = "PRAGMA foreign_keys = OFF;";
                pragmaCmd.ExecuteNonQuery();
            }

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();

            return (context, connection);
        }

        [Fact]
        public async Task DefinirMetaAnualAsync_QuandoDadosValidos_DeveCriarNovaMeta()
        {
            // Arrange
            var (context, connection) = CriarContextoSqliteEmMemoria();
            using var _ = connection;
            using var __ = context;

            var repository = new MetaAnualRepository(context);
            var service = new MetaAnualService(repository);
            var dto = new DefinirMetaAnualDto { QuantidadeMeta = 15 };

            // Act
            var resultado = await service.DefinirMetaAnualAsync(1, 2026, dto);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.UsuarioId);
            Assert.Equal(2026, resultado.Ano);
            Assert.Equal(15, resultado.QuantidadeMeta);
            Assert.Equal(1, await context.MetasAnuais.CountAsync());
        }

        [Fact]
        public async Task DefinirMetaAnualAsync_QuandoMetaJaExisteNoAno_DeveAtualizarSemDuplicar()
        {
            // Arrange
            var (context, connection) = CriarContextoSqliteEmMemoria();
            using var _ = connection;
            using var __ = context;

            var repository = new MetaAnualRepository(context);
            var service = new MetaAnualService(repository);

            var primeiraMeta = await service.DefinirMetaAnualAsync(1, 2026, new DefinirMetaAnualDto { QuantidadeMeta = 10 });

            // Act
            var metaAtualizada = await service.DefinirMetaAnualAsync(1, 2026, new DefinirMetaAnualDto { QuantidadeMeta = 24 });

            // Assert
            Assert.Equal(primeiraMeta.Id, metaAtualizada.Id);
            Assert.Equal(24, metaAtualizada.QuantidadeMeta);
            Assert.Equal(1, await context.MetasAnuais.CountAsync());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task DefinirMetaAnualAsync_QuandoQuantidadeZeroOuNegativa_DeveLancarArgumentException(int quantidadeInvalida)
        {
            // Arrange
            var (context, connection) = CriarContextoSqliteEmMemoria();
            using var _ = connection;
            using var __ = context;

            var repository = new MetaAnualRepository(context);
            var service = new MetaAnualService(repository);
            var dto = new DefinirMetaAnualDto { QuantidadeMeta = quantidadeInvalida };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.DefinirMetaAnualAsync(1, 2026, dto));
        }
    }
}