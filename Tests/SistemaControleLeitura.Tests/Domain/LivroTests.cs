using SistemaControleLeitura.Domain.Entities;

namespace SistemaControleLeitura.Tests.Domain
{
    public class LivroTests
    {
        // ---------- Construtor ----------

        [Fact]
        public void Construtor_QuandoDadosValidos_DeveCriarLivro()
        {
            var livro = new Livro(1, "Dom Casmurro", "Machado de Assis", 256);

            Assert.Equal(1, livro.UsuarioId);
            Assert.Equal("Dom Casmurro", livro.Titulo);
            Assert.Equal("Machado de Assis", livro.Autor);
            Assert.Equal(256, livro.NumeroPaginas);
        }

        [Fact]
        public void Construtor_QuandoTextosTemEspacosNasPontas_DeveGuardarSemEspacos()
        {
            var livro = new Livro(1, "  Dom Casmurro  ", "  Machado de Assis ", 256);

            Assert.Equal("Dom Casmurro", livro.Titulo);
            Assert.Equal("Machado de Assis", livro.Autor);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("a")]
        [InlineData(" a ")]
        public void Construtor_QuandoTituloInvalido_DeveLancarArgumentException(string? titulo)
        {
            var erro = Assert.Throws<ArgumentException>(() => new Livro(1, titulo!, "Machado de Assis", 256));

            Assert.Equal("O título deve conter entre 2 e 120 caracteres.", erro.Message);
        }

        [Fact]
        public void Construtor_QuandoTituloTemMaisDe120Caracteres_DeveLancarArgumentException()
        {
            var tituloLongo = new string('a', 121);

            Assert.Throws<ArgumentException>(() => new Livro(1, tituloLongo, "Machado de Assis", 256));
        }

        [Fact]
        public void Construtor_QuandoTituloTemExatamente120Caracteres_DeveCriarLivro()
        {
            var titulo = new string('a', 120);

            var livro = new Livro(1, titulo, "Machado de Assis", 256);

            Assert.Equal(120, livro.Titulo.Length);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("a")]
        public void Construtor_QuandoAutorInvalido_DeveLancarArgumentException(string? autor)
        {
            var erro = Assert.Throws<ArgumentException>(() => new Livro(1, "Dom Casmurro", autor!, 256));

            Assert.Equal("O autor deve conter entre 2 e 120 caracteres.", erro.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Construtor_QuandoNumeroPaginasNaoPositivo_DeveLancarArgumentException(int paginas)
        {
            var erro = Assert.Throws<ArgumentException>(() => new Livro(1, "Dom Casmurro", "Machado de Assis", paginas));

            Assert.Equal("O número de páginas deve ser maior que zero.", erro.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Construtor_QuandoUsuarioIdNaoPositivo_DeveLancarArgumentException(int usuarioId)
        {
            Assert.Throws<ArgumentException>(() => new Livro(usuarioId, "Dom Casmurro", "Machado de Assis", 256));
        }

        // ---------- Atualizar ----------

        [Fact]
        public void Atualizar_QuandoDadosValidos_DeveAlterarCampos()
        {
            var livro = new Livro(1, "Dom Casmurro", "Machado de Assis", 256);

            livro.Atualizar("  Memórias Póstumas  ", "Machado", 300);

            Assert.Equal("Memórias Póstumas", livro.Titulo);
            Assert.Equal("Machado", livro.Autor);
            Assert.Equal(300, livro.NumeroPaginas);
            Assert.Equal(1, livro.UsuarioId); // o dono não muda
        }

        [Fact]
        public void Atualizar_QuandoDadosInvalidos_DeveLancarExcecaoESemAlterarNada()
        {
            var livro = new Livro(1, "Dom Casmurro", "Machado de Assis", 256);

            // título válido, mas páginas inválidas: nenhum campo pode mudar
            Assert.Throws<ArgumentException>(() => livro.Atualizar("Outro título", "Outro autor", 0));

            Assert.Equal("Dom Casmurro", livro.Titulo);
            Assert.Equal("Machado de Assis", livro.Autor);
            Assert.Equal(256, livro.NumeroPaginas);
        }
    }
}
