using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Services;
using SistemaControleLeitura.Domain.Entities;
using SistemaControleLeitura.Domain.Enums;
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

        [Theory]
        [InlineData(1999)]
        [InlineData(2101)]
        public async Task DefinirMetaAnualAsync_QuandoAnoInvalido_DeveLancarArgumentException(int anoInvalido)
        {
            // Arrange
            var (context, connection) = CriarContextoSqliteEmMemoria();
            using var _ = connection;
            using var __ = context;

            var repository = new MetaAnualRepository(context);
            var service = new MetaAnualService(repository);
            var dto = new DefinirMetaAnualDto { QuantidadeMeta = 12 };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.DefinirMetaAnualAsync(1, anoInvalido, dto));
        }

        [Fact]
        public async Task ConsultarProgressoAsync_QuandoExistemLivrosLidosNoAno_DeveCalcularPercentualEMediaCorretamente()
        {
            // Arrange
            var (context, connection) = CriarContextoSqliteEmMemoria();
            using var _ = connection;
            using var __ = context;

            var repository = new MetaAnualRepository(context);
            var service = new MetaAnualService(repository);

            int anoAtual = DateTime.Now.Year;
            await service.DefinirMetaAnualAsync(1, anoAtual, new DefinirMetaAnualDto { QuantidadeMeta = 10 });

            // Livro 1: Lido no ano atual (deve contar)
            var livro1 = new Livro(1, "Livro 1", "Autor 1", 200);
            context.Livros.Add(livro1);
            await context.SaveChangesAsync();

            var item1 = new ItemEstante { LivroId = livro1.Id, DataAdicao = new DateTime(anoAtual, 1, 10) };
            context.ItensEstante.Add(item1);
            await context.SaveChangesAsync();

            context.Leituras.Add(new Leitura
            {
                ItemEstanteId = item1.Id,
                Status = StatusLeitura.Lido,
                DataInicio = new DateTime(anoAtual, 2, 1),
                DataTermino = new DateTime(anoAtual, 2, 15)
            });

            // Livro 2: Lido em ano anterior (NÃO deve contar)
            var livro2 = new Livro(1, "Livro 2", "Autor 2", 150);
            context.Livros.Add(livro2);
            await context.SaveChangesAsync();

            var item2 = new ItemEstante { LivroId = livro2.Id, DataAdicao = new DateTime(anoAtual - 1, 5, 1) };
            context.ItensEstante.Add(item2);
            await context.SaveChangesAsync();

            context.Leituras.Add(new Leitura
            {
                ItemEstanteId = item2.Id,
                Status = StatusLeitura.Lido,
                DataInicio = new DateTime(anoAtual - 1, 5, 2),
                DataTermino = new DateTime(anoAtual - 1, 5, 20)
            });

            // Livro 3: Status Lendo no ano atual (NÃO deve contar)
            var livro3 = new Livro(1, "Livro 3", "Autor 3", 300);
            context.Livros.Add(livro3);
            await context.SaveChangesAsync();

            var item3 = new ItemEstante { LivroId = livro3.Id, DataAdicao = new DateTime(anoAtual, 3, 1) };
            context.ItensEstante.Add(item3);
            await context.SaveChangesAsync();

            context.Leituras.Add(new Leitura
            {
                ItemEstanteId = item3.Id,
                Status = StatusLeitura.Lendo,
                DataInicio = new DateTime(anoAtual, 3, 5),
                DataTermino = null
            });

            // Livro 4: Lido no ano atual, mas de OUTRO usuário (UsuarioId = 2 - NÃO deve contar)
            var livroOutroUsuario = new Livro(2, "Livro Alheio", "Autor 4", 180);
            context.Livros.Add(livroOutroUsuario);
            await context.SaveChangesAsync();

            var itemOutroUsuario = new ItemEstante { LivroId = livroOutroUsuario.Id, DataAdicao = new DateTime(anoAtual, 4, 1) };
            context.ItensEstante.Add(itemOutroUsuario);
            await context.SaveChangesAsync();

            context.Leituras.Add(new Leitura
            {
                ItemEstanteId = itemOutroUsuario.Id,
                Status = StatusLeitura.Lido,
                DataInicio = new DateTime(anoAtual, 4, 2),
                DataTermino = new DateTime(anoAtual, 4, 10)
            });

            await context.SaveChangesAsync();

            int mesesRestantes = (12 - DateTime.Now.Month) + 1;
            double mediaEsperada = Math.Round(9.0 / mesesRestantes, 1);

            // Act
            var progresso = await service.ConsultarProgressoAsync(1, anoAtual);

            // Assert
            Assert.NotNull(progresso);
            Assert.Equal(anoAtual, progresso.Ano);
            Assert.Equal(10, progresso.QuantidadeMeta);
            Assert.Equal(1, progresso.LivrosLidos);
            Assert.Equal(9, progresso.LivrosRestantes);
            Assert.Equal(10.0, progresso.PercentualConcluido);
            Assert.Equal(mediaEsperada, progresso.MediaMensalNecessaria);
        }

        [Fact]
        public async Task ConsultarProgressoAsync_QuandoMetaUltrapassada_DevePermitirPercentualAcimaDeCemEMediaZero()
        {
            // Arrange
            var (context, connection) = CriarContextoSqliteEmMemoria();
            using var _ = connection;
            using var __ = context;

            var repository = new MetaAnualRepository(context);
            var service = new MetaAnualService(repository);

            await service.DefinirMetaAnualAsync(1, 2026, new DefinirMetaAnualDto { QuantidadeMeta = 2 });

            for (int i = 1; i <= 3; i++)
            {
                var livro = new Livro(1, $"Livro {i}", "Autor", 100);
                context.Livros.Add(livro);
                await context.SaveChangesAsync();

                var item = new ItemEstante { LivroId = livro.Id, DataAdicao = new DateTime(2026, 1, i) };
                context.ItensEstante.Add(item);
                await context.SaveChangesAsync();

                context.Leituras.Add(new Leitura
                {
                    ItemEstanteId = item.Id,
                    Status = StatusLeitura.Lido,
                    DataInicio = new DateTime(2026, 1, i),
                    DataTermino = new DateTime(2026, 2, i)
                });
            }

            await context.SaveChangesAsync();

            // Act
            var progresso = await service.ConsultarProgressoAsync(1, 2026);

            // Assert
            Assert.Equal(3, progresso.LivrosLidos);
            Assert.Equal(0, progresso.LivrosRestantes);
            Assert.Equal(150.0, progresso.PercentualConcluido);
            Assert.Equal(0.0, progresso.MediaMensalNecessaria);
        }

        [Fact]
        public async Task ConsultarProgressoAsync_QuandoMetaNaoExisteOuPertenceAOutroUsuario_DeveLancarKeyNotFoundException()
        {
            // Arrange
            var (context, connection) = CriarContextoSqliteEmMemoria();
            using var _ = connection;
            using var __ = context;

            var repository = new MetaAnualRepository(context);
            var service = new MetaAnualService(repository);

            // Usuário 2 cadastra uma meta para 2026, mas Usuário 1 tenta consultar
            await service.DefinirMetaAnualAsync(2, 2026, new DefinirMetaAnualDto { QuantidadeMeta = 12 });

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.ConsultarProgressoAsync(1, 2026));
        }

        [Theory]
        [InlineData(1999)]
        [InlineData(2101)]
        public async Task ConsultarProgressoAsync_QuandoAnoInvalido_DeveLancarArgumentException(int anoInvalido)
        {
            // Arrange
            var (context, connection) = CriarContextoSqliteEmMemoria();
            using var _ = connection;
            using var __ = context;

            var repository = new MetaAnualRepository(context);
            var service = new MetaAnualService(repository);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.ConsultarProgressoAsync(1, anoInvalido));
        }
    }
}