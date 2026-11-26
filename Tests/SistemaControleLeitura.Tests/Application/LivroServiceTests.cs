using Moq;
using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Domain.Interfaces;
using SistemaControleLeitura.Application.Services;
using SistemaControleLeitura.Domain.Entities;

namespace SistemaControleLeitura.Tests.Application
{
    public class LivroServiceTests
    {
        // O repository é "falso" (Moq): o teste controla o que ele devolve
        // e confere se o Service o chamou do jeito certo, sem precisar de banco.
        private readonly Mock<ILivroRepository> _repositoryMock = new();
        private readonly LivroService _service;

        public LivroServiceTests()
        {
            _service = new LivroService(_repositoryMock.Object);
        }

        private static Livro NovoLivro(int usuarioId = 1) =>
            new Livro(usuarioId, "Dom Casmurro", "Machado de Assis", 256);

        // ---------- Cadastrar ----------

        [Fact]
        public async Task CadastrarLivroAsync_QuandoDadosValidos_DeveGravarEDevolverDto()
        {
            Livro? livroGravado = null;
            _repositoryMock
                .Setup(r => r.CadastrarLivroAsync(It.IsAny<Livro>(), It.IsAny<CancellationToken>()))
                .Callback<Livro, CancellationToken>((livro, _) => livroGravado = livro)
                .Returns(Task.CompletedTask);

            var dto = new LivroCadastroDto { Titulo = "  Dom Casmurro ", Autor = "Machado de Assis", NumeroPaginas = 256 };

            var resultado = await _service.CadastrarLivroAsync(1, dto);

            Assert.NotNull(livroGravado);
            Assert.Equal(1, livroGravado!.UsuarioId);
            Assert.Equal("Dom Casmurro", resultado.Titulo);
            Assert.Equal("Machado de Assis", resultado.Autor);
            Assert.Equal(256, resultado.NumeroPaginas);
            _repositoryMock.Verify(
                r => r.CadastrarLivroAsync(It.IsAny<Livro>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CadastrarLivroAsync_QuandoDadosInvalidos_DeveLancarExcecaoESemGravar()
        {
            var dto = new LivroCadastroDto { Titulo = "a", Autor = "Machado de Assis", NumeroPaginas = 256 };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CadastrarLivroAsync(1, dto));

            _repositoryMock.Verify(
                r => r.CadastrarLivroAsync(It.IsAny<Livro>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // ---------- Listar ----------

        [Fact]
        public async Task ListarLivrosAsync_DeveDevolverSomenteOsDadosDoRepositorioConvertidosParaDto()
        {
            _repositoryMock
                .Setup(r => r.ListarPorUsuarioAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Livro> { NovoLivro(), new Livro(1, "Quincas Borba", "Machado de Assis", 300) });

            var resultado = await _service.ListarLivrosAsync(1);

            Assert.Equal(2, resultado.Count);
            Assert.Equal("Dom Casmurro", resultado[0].Titulo);
            Assert.Equal("Quincas Borba", resultado[1].Titulo);
        }

        [Fact]
        public async Task ListarLivrosAsync_QuandoNaoHaLivros_DeveDevolverListaVazia()
        {
            _repositoryMock
                .Setup(r => r.ListarPorUsuarioAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Livro>());

            var resultado = await _service.ListarLivrosAsync(1);

            Assert.Empty(resultado);
        }

        // ---------- Obter por id ----------

        [Fact]
        public async Task ObterLivroPorIdAsync_QuandoExiste_DeveDevolverDto()
        {
            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(1, 5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(NovoLivro());

            var resultado = await _service.ObterLivroPorIdAsync(1, 5);

            Assert.NotNull(resultado);
            Assert.Equal("Dom Casmurro", resultado!.Titulo);
        }

        [Fact]
        public async Task ObterLivroPorIdAsync_QuandoNaoExisteOuEhDeOutraUsuaria_DeveDevolverNull()
        {
            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(1, 99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Livro?)null);

            var resultado = await _service.ObterLivroPorIdAsync(1, 99);

            Assert.Null(resultado);
        }

        // ---------- Atualizar ----------

        [Fact]
        public async Task AtualizarLivroAsync_QuandoLivroNaoExiste_DeveDevolverNullESemSalvar()
        {
            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(1, 99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Livro?)null);

            var dto = new LivroCadastroDto { Titulo = "Novo", Autor = "Autor", NumeroPaginas = 10 };

            var resultado = await _service.AtualizarLivroAsync(1, 99, dto);

            Assert.Null(resultado);
            _repositoryMock.Verify(
                r => r.AtualizarAsync(It.IsAny<Livro>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarLivroAsync_QuandoDadosValidos_DeveAlterarESalvar()
        {
            var livro = NovoLivro();
            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(1, 5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(livro);

            var dto = new LivroCadastroDto { Titulo = "Memórias Póstumas", Autor = "Machado", NumeroPaginas = 300 };

            var resultado = await _service.AtualizarLivroAsync(1, 5, dto);

            Assert.NotNull(resultado);
            Assert.Equal("Memórias Póstumas", resultado!.Titulo);
            Assert.Equal(300, resultado.NumeroPaginas);
            _repositoryMock.Verify(r => r.AtualizarAsync(livro, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarLivroAsync_QuandoDadosInvalidos_DeveLancarExcecaoESemSalvar()
        {
            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(1, 5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(NovoLivro());

            var dto = new LivroCadastroDto { Titulo = "Novo", Autor = "Autor", NumeroPaginas = 0 };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.AtualizarLivroAsync(1, 5, dto));

            _repositoryMock.Verify(
                r => r.AtualizarAsync(It.IsAny<Livro>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // ---------- Excluir ----------

        [Fact]
        public async Task ExcluirLivroAsync_QuandoLivroNaoExiste_DeveDevolverNaoEncontrado()
        {
            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(1, 99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Livro?)null);

            var resultado = await _service.ExcluirLivroAsync(1, 99);

            Assert.Equal(ResultadoExclusaoLivro.NaoEncontrado, resultado);
            _repositoryMock.Verify(
                r => r.RemoverAsync(It.IsAny<Livro>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExcluirLivroAsync_QuandoLivroEstaNaEstante_DeveRecusarESemRemover()
        {
            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(1, 5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(NovoLivro());
            _repositoryMock
                .Setup(r => r.LivroEstaNaEstanteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resultado = await _service.ExcluirLivroAsync(1, 5);

            Assert.Equal(ResultadoExclusaoLivro.NaEstante, resultado);
            _repositoryMock.Verify(
                r => r.RemoverAsync(It.IsAny<Livro>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExcluirLivroAsync_QuandoLivroForaDaEstante_DeveRemover()
        {
            var livro = NovoLivro();
            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(1, 5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(livro);
            _repositoryMock
                .Setup(r => r.LivroEstaNaEstanteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var resultado = await _service.ExcluirLivroAsync(1, 5);

            Assert.Equal(ResultadoExclusaoLivro.Excluido, resultado);
            _repositoryMock.Verify(r => r.RemoverAsync(livro, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
