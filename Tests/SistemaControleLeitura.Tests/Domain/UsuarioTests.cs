using SistemaControleLeitura.Domain.Entities;

namespace SistemaControleLeitura.Tests.Domain {
    public class UsuarioTests {

        [Fact]
        public void Construtor_QuandoDadosValidos_DeveCriarUsuario() {

            var nome = "Maria";
            var email = "maria@gmail.com";
            var senhaHash = "hashSenha123";

            var usuario = new Usuario(nome, email, senhaHash);

            Assert.Equal(nome, usuario.Nome);
            Assert.Equal(email, usuario.Email);
            Assert.Equal(senhaHash, usuario.SenhaHash);
        }

        [Fact]
        public void Construtor_QuandoNomeInvalido_DeveLancarExcecao() {

            var nome = "";
            var email = "maria@gmail.com";
            var senhaHash = "hashSenha123";

            var excecao = Assert.Throws<ArgumentException>(
                () => new Usuario(nome, email, senhaHash)
            );

            Assert.Equal("Nome informado é inválido.", excecao.Message);
        }

        [Fact]
        public void Construtor_QuandoEmailInvalido_DeveLancarExcecao() {

            var nome = "Maria";
            var email = "";
            var senhaHash = "hashSenha123";

            var excecao = Assert.Throws<ArgumentException>(
                () => new Usuario(nome, email, senhaHash)
            );

            Assert.Equal("Email informado é inválido.", excecao.Message);
        }

        [Fact]
        public void Construtor_QuandoSenhaHashInvalida_DeveLancarExcecao() {

            var nome = "Maria";
            var email = "maria@gmail.com";
            var senhaHash = "";

            var excecao = Assert.Throws<ArgumentException>(
                () => new Usuario(nome, email, senhaHash)
            );

            Assert.Equal("Hash da senha informado é inválido.", excecao.Message);
        }
    }
}
