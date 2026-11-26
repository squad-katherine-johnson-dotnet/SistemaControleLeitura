
using Moq;
using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Services;
using SistemaControleLeitura.Domain.Entities;
using SistemaControleLeitura.Domain.Enums;
using SistemaControleLeitura.Domain.Interfaces;

namespace SistemaControleLeitura.Tests.Application;

public class EstanteServiceTests
{
    private readonly Mock<IEstanteRepository> _repositoryMock;
    private readonly EstanteService _service;

    public EstanteServiceTests()
    {
        _repositoryMock = new Mock<IEstanteRepository>();
        _service = new EstanteService(_repositoryMock.Object);
    }

    private static Livro CriarLivro() {
        var livro = new Livro(
            1,
            "Dom Casmurro",
            "Machado de Assis",
            256
        );

        typeof(Livro)
            .GetProperty(nameof(Livro.Id))!
            .SetValue(livro, 1);

        return livro;
    }

    private static Leitura CriarLeitura(
        StatusLeitura status = StatusLeitura.QueroLer)
    {
        return new Leitura
        {
            Id = 1,
            ItemEstanteId = 1,
            Status = status
        };
    }

    [Fact]
    public async Task AdicionarLivroAsync_DeveAdicionarLivroComSucesso() {
        ItemEstante? itemCriado = null;
        Leitura? leituraCriada = null;

        _repositoryMock
            .Setup(r => r.ObterLivroDoUsuarioAsync(1, 1))
            .ReturnsAsync(CriarLivro());

        _repositoryMock
            .Setup(r => r.LivroJaEstaNaEstanteAsync(1))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.AdicionarItemComLeituraAsync(
                It.IsAny<ItemEstante>(),
                It.IsAny<Leitura>()))
            .Callback<ItemEstante, Leitura>((item, leitura) => {
                item.Id = 10;
                leitura.Id = 20;

                itemCriado = item;
                leituraCriada = leitura;
            })
            .Returns(Task.CompletedTask);

        var resultado = await _service.AdicionarLivroAsync(1, 1);

        Assert.NotNull(itemCriado);
        Assert.Equal(1, itemCriado.LivroId);

        Assert.NotNull(leituraCriada);
        Assert.Equal(StatusLeitura.QueroLer, leituraCriada.Status);

        Assert.Equal(10, resultado.ItemEstanteId);
        Assert.Equal(1, resultado.LivroId);
        Assert.Equal(20, resultado.LeituraId);
        Assert.Equal("Dom Casmurro", resultado.Titulo);
        Assert.Equal("Machado de Assis", resultado.Autor);
        Assert.Equal("Quero ler", resultado.Status);

        _repositoryMock.Verify(
            r => r.AdicionarItemComLeituraAsync(
                It.IsAny<ItemEstante>(),
                It.IsAny<Leitura>()),
            Times.Once);
    }

    [Fact]
    public async Task AdicionarLivroAsync_ComIdInvalido_DeveLancarExcecao()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.AdicionarLivroAsync(0, 1));
    }

    [Fact]
    public async Task AdicionarLivroAsync_ComLivroInexistente_DeveLancarExcecao()
    {
        _repositoryMock
            .Setup(r => r.ObterLivroDoUsuarioAsync(99, 1))
            .ReturnsAsync((Livro?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.AdicionarLivroAsync(99, 1));
    }

    [Fact]
    public async Task AdicionarLivroAsync_ComLivroDuplicado_DeveLancarExcecao()
    {
        _repositoryMock
            .Setup(r => r.ObterLivroDoUsuarioAsync(1, 1))
            .ReturnsAsync(CriarLivro());

        _repositoryMock
            .Setup(r => r.LivroJaEstaNaEstanteAsync(1))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.AdicionarLivroAsync(1, 1));
    }

    [Fact]
    public async Task AlterarStatusAsync_DeveAlterarParaLendo() {
        var leitura = CriarLeitura();
        var item = new ItemEstante { Id = 1, LivroId = 1 };
        var livro = CriarLivro();

        var dataInicio = new DateTime(2026, 1, 10);

        _repositoryMock
            .Setup(r => r.ObterLeituraDoUsuarioAsync(1, 1))
            .ReturnsAsync(leitura);

        _repositoryMock
            .Setup(r => r.SalvarAlteracoesAsync())
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.ObterItemComUltimaLeituraAsync(1, 1))
            .ReturnsAsync((item, livro, leitura));

        var dto = new AtualizarStatusLeituraDto {
            Status = "Lendo",
            DataInicio = dataInicio
        };

        var resultado = await _service.AlterarStatusAsync(1, 1, dto);

        Assert.Equal(StatusLeitura.Lendo, leitura.Status);
        Assert.Equal(dataInicio, leitura.DataInicio);

        Assert.Equal(1, resultado.ItemEstanteId);
        Assert.Equal(1, resultado.LivroId);
        Assert.Equal(1, resultado.LeituraId);
        Assert.Equal("Lendo", resultado.Status);
        Assert.Equal("Dom Casmurro", resultado.Titulo);
        Assert.Equal(dataInicio, resultado.DataInicio);

        _repositoryMock.Verify(
            r => r.SalvarAlteracoesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task AlterarStatusAsync_ComStatusInvalido_DeveLancarExcecao()
    {
        var dto = new AtualizarStatusLeituraDto
        {
            Status = "Invalido"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.AlterarStatusAsync(1, 1, dto));
    }

    [Fact]
    public async Task AlterarStatusAsync_ComLeituraInexistente_DeveLancarExcecao()
    {
        _repositoryMock
            .Setup(r => r.ObterLeituraDoUsuarioAsync(99, 1))
            .ReturnsAsync((Leitura?)null);

        var dto = new AtualizarStatusLeituraDto
        {
            Status = "Lendo",
            DataInicio = DateTime.Today
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.AlterarStatusAsync(99, 1, dto));
    }

    [Fact]
    public async Task AlterarStatusAsync_AoPularEtapa_DeveLancarExcecao()
    {
        var leitura = CriarLeitura(StatusLeitura.QueroLer);

        _repositoryMock
            .Setup(r => r.ObterLeituraDoUsuarioAsync(1, 1))
            .ReturnsAsync(leitura);

        var dto = new AtualizarStatusLeituraDto
        {
            Status = "Lido",
            DataInicio = DateTime.Today,
            DataTermino = DateTime.Today
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.AlterarStatusAsync(1, 1, dto));
    }

    [Fact]
    public async Task AlterarStatusAsync_ComDataInicioAusente_DeveLancarExcecao()
    {
        var leitura = CriarLeitura();

        _repositoryMock
            .Setup(r => r.ObterLeituraDoUsuarioAsync(1, 1))
            .ReturnsAsync(leitura);

        var dto = new AtualizarStatusLeituraDto
        {
            Status = "Lendo"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.AlterarStatusAsync(1, 1, dto));
    }

    [Fact]
    public async Task AlterarStatusAsync_DeveConcluirLeituraComSucesso() {
        var dataInicio = new DateTime(2026, 1, 10);
        var dataTermino = new DateTime(2026, 1, 20);

        var leitura = CriarLeitura(StatusLeitura.Lendo);
        leitura.DataInicio = dataInicio;

        var item = new ItemEstante { Id = 1, LivroId = 1 };
        var livro = CriarLivro();

        _repositoryMock
            .Setup(r => r.ObterLeituraDoUsuarioAsync(1, 1))
            .ReturnsAsync(leitura);

        _repositoryMock
            .Setup(r => r.SalvarAlteracoesAsync())
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.ObterItemComUltimaLeituraAsync(1, 1))
            .ReturnsAsync((item, livro, leitura));

        var dto = new AtualizarStatusLeituraDto {
            Status = "Lido",
            DataTermino = dataTermino
        };

        var resultado = await _service.AlterarStatusAsync(1, 1, dto);

        Assert.Equal(StatusLeitura.Lido, leitura.Status);
        Assert.Equal(dataTermino, leitura.DataTermino);

        Assert.Equal(1, resultado.ItemEstanteId);
        Assert.Equal(1, resultado.LivroId);
        Assert.Equal(1, resultado.LeituraId);
        Assert.Equal("Lido", resultado.Status);
        Assert.Equal("Dom Casmurro", resultado.Titulo);
        Assert.Equal(dataInicio, resultado.DataInicio);
        Assert.Equal(dataTermino, resultado.DataTermino);
    }

    [Fact]
    public async Task AlterarStatusAsync_ComDataTerminoAusente_DeveLancarExcecao()
    {
        var leitura = CriarLeitura(StatusLeitura.Lendo);
        leitura.DataInicio = new DateTime(2026, 1, 10);

        _repositoryMock
            .Setup(r => r.ObterLeituraDoUsuarioAsync(1, 1))
            .ReturnsAsync(leitura);

        var dto = new AtualizarStatusLeituraDto
        {
            Status = "Lido"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.AlterarStatusAsync(1, 1, dto));
    }

    [Fact]
    public async Task AlterarStatusAsync_ComDataTerminoAnteriorAoInicio_DeveLancarExcecao()
    {
        var leitura = CriarLeitura(StatusLeitura.Lendo);
        leitura.DataInicio = new DateTime(2026, 1, 20);

        _repositoryMock
            .Setup(r => r.ObterLeituraDoUsuarioAsync(1, 1))
            .ReturnsAsync(leitura);

        var dto = new AtualizarStatusLeituraDto
        {
            Status = "Lido",
            DataTermino = new DateTime(2026, 1, 10)
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.AlterarStatusAsync(1, 1, dto));
    }

    [Fact]
    public async Task AlterarStatusAsync_LeituraConcluida_NaoDeveVoltarStatus()
    {
        var leitura = CriarLeitura(StatusLeitura.Lido);

        _repositoryMock
            .Setup(r => r.ObterLeituraDoUsuarioAsync(1, 1))
            .ReturnsAsync(leitura);

        var dto = new AtualizarStatusLeituraDto
        {
            Status = "Lendo"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.AlterarStatusAsync(1, 1, dto));
    }

    [Fact]
    public async Task IniciarReleituraAsync_ComLeituraConcluida_DeveCriarNovaLeitura() {
        var item = new ItemEstante { Id = 1, LivroId = 1 };
        var livro = CriarLivro();
        var ultimaLeitura = CriarLeitura(StatusLeitura.Lido);

        Leitura? novaLeituraCriada = null;

        _repositoryMock
            .Setup(r => r.ObterItemComUltimaLeituraAsync(1, 1))
            .ReturnsAsync((item, livro, ultimaLeitura));

        _repositoryMock
            .Setup(r => r.AdicionarNovaLeituraAsync(It.IsAny<Leitura>()))
            .Callback<Leitura>(leitura => {
                leitura.Id = 2;
                novaLeituraCriada = leitura;
            })
            .Returns(Task.CompletedTask);

        var resultado = await _service.IniciarReleituraAsync(1, 1);

        Assert.NotNull(novaLeituraCriada);
        Assert.Equal(1, novaLeituraCriada.ItemEstanteId);
        Assert.Equal(StatusLeitura.QueroLer, novaLeituraCriada.Status);

        Assert.Equal(1, resultado.ItemEstanteId);
        Assert.Equal(1, resultado.LivroId);
        Assert.Equal(2, resultado.LeituraId);
        Assert.Equal("Dom Casmurro", resultado.Titulo);
        Assert.Equal("Quero ler", resultado.Status);

        _repositoryMock.Verify(
            r => r.AdicionarNovaLeituraAsync(It.IsAny<Leitura>()),
            Times.Once);
    }

    [Fact]
    public async Task IniciarReleituraAsync_ComLeituraNaoConcluida_DeveLancarExcecao()
    {
        var item = new ItemEstante { Id = 1, LivroId = 1 };
        var livro = CriarLivro();
        var leitura = CriarLeitura(StatusLeitura.Lendo);

        _repositoryMock
            .Setup(r => r.ObterItemComUltimaLeituraAsync(1, 1))
            .ReturnsAsync((item, livro, leitura));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.IniciarReleituraAsync(1, 1));
    }

    [Fact]
    public async Task IniciarReleituraAsync_ComItemInexistente_DeveLancarExcecao()
    {
        _repositoryMock
            .Setup(r => r.ObterItemComUltimaLeituraAsync(99, 1))
            .ReturnsAsync(((ItemEstante?)null, (Livro?)null, (Leitura?)null));

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.IniciarReleituraAsync(99, 1));
    }

    [Fact]
    public async Task ListarEstanteAsync_DeveRetornarLivros() {
        var itens = new List<(ItemEstante Item, Livro Livro, Leitura UltimaLeitura)>
        {
        (
            new ItemEstante
            {
                Id = 1,
                LivroId = 1,
                DataAdicao = DateTime.Today
            },
            CriarLivro(),
            CriarLeitura()
        )
    };

        _repositoryMock
            .Setup(r => r.ListarEstanteAsync(1, null, null))
            .ReturnsAsync(itens);

        var resultado = await _service.ListarEstanteAsync(1, null, null);

        Assert.Single(resultado);
        Assert.Equal("Dom Casmurro", resultado[0].Titulo);
    }

    [Fact]
    public async Task ListarEstanteAsync_ComStatusInvalido_DeveLancarExcecao()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.ListarEstanteAsync(1, "Invalido", null));
    }

    [Fact]
    public async Task ListarEstanteAsync_ComAnoInvalido_DeveLancarExcecao()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.ListarEstanteAsync(1, null, 0));
    }

    [Fact]
    public async Task RemoverLivroAsync_DeveRemoverItemDaEstante()
    {
        var item = new ItemEstante { Id = 1, LivroId = 1 };
        var livro = CriarLivro();
        var leitura = CriarLeitura(StatusLeitura.QueroLer);

        _repositoryMock
            .Setup(r => r.ObterItemComUltimaLeituraAsync(1, 1))
            .ReturnsAsync((item, livro, leitura));

        _repositoryMock
            .Setup(r => r.RemoverItemAsync(item))
            .Returns(Task.CompletedTask);

        await _service.RemoverLivroAsync(1, 1);

        _repositoryMock.Verify(
            r => r.RemoverItemAsync(item),
            Times.Once);
    }

    [Fact]
    public async Task RemoverLivroAsync_ComItemInexistente_DeveLancarExcecao()
    {
        _repositoryMock
            .Setup(r => r.ObterItemComUltimaLeituraAsync(99, 1))
            .ReturnsAsync(((ItemEstante?)null, (Livro?)null, (Leitura?)null));

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.RemoverLivroAsync(99, 1));
    }
}