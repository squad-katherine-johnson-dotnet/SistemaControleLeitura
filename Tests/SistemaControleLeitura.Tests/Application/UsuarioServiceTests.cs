using Microsoft.AspNetCore.Identity;
using Moq;
using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Interfaces;
using SistemaControleLeitura.Application.Services;
using SistemaControleLeitura.Domain.Entities;

namespace SistemaControleLeitura.Tests.Application {
    public class UsuarioServiceTests {

        private readonly Mock<IUsuarioRepository> _repositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests() {
            _repositoryMock = new Mock<IUsuarioRepository>();
            _tokenServiceMock = new Mock<ITokenService>();

            _usuarioService = new UsuarioService(_repositoryMock.Object, _tokenServiceMock.Object);
        }

        [Fact]
        public async Task CadastrarUsuarioAsync_QuandoDadosValidos_DeveCadastrarUsuario() {

            var dto = new UsuarioCadastroDto {
                Nome = "Maria",
                Email = "maria@gmail.com",
                Senha = "Senha123!"
            };

            Usuario? usuarioCadastrado = null;

            _repositoryMock
                .Setup(r => r.ExisteEmailAsync(dto.Email.Trim().ToLowerInvariant(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _repositoryMock
                .Setup(r => r.CadastrarUsuarioAsync(
                    It.IsAny<Usuario>(),
                    It.IsAny<CancellationToken>()))
                .Callback<Usuario, CancellationToken>((usuario, _) => {
                    usuarioCadastrado = usuario;
                })
                .Returns(Task.CompletedTask);

            var resultado = await _usuarioService.CadastrarUsuarioAsync(dto);

            Assert.NotNull(usuarioCadastrado);

            Assert.NotEqual(dto.Senha, usuarioCadastrado.SenhaHash);

            var passwordHasher = new PasswordHasher<Usuario>();

            var verificacaoSenha = passwordHasher.VerifyHashedPassword(
                usuarioCadastrado,
                usuarioCadastrado.SenhaHash,
                dto.Senha
                );

            Assert.Equal(PasswordVerificationResult.Success, verificacaoSenha);

            Assert.NotNull(resultado);
            Assert.Equal(dto.Nome, resultado.Nome);
            Assert.Equal(dto.Email.Trim().ToLowerInvariant(), resultado.Email);
            Assert.Equal(dto.Email.Trim().ToLowerInvariant(), usuarioCadastrado.Email);

            _repositoryMock.Verify(r => r.CadastrarUsuarioAsync(
                It.IsAny<Usuario>(),
                It.IsAny<CancellationToken>()),
                Times.Once
                );

            _repositoryMock.Verify(r => r.ExisteEmailAsync(
                dto.Email.Trim().ToLowerInvariant(), It.IsAny<CancellationToken>()),
                Times.Once
                );
        }

        [Fact]
        public async Task CadastrarUsuarioAsync_QuandoEmailJaCadastrado_DeveLancarExcecao() {

            var dto = new UsuarioCadastroDto {
                Nome = "Maria",
                Email = "maria@gmail.com",
                Senha = "Senha123!"
            };

            _repositoryMock
                .Setup(r => r.ExisteEmailAsync(dto.Email.Trim().ToLowerInvariant()))
                .ReturnsAsync(true);

            var excecao = await Assert.ThrowsAsync<ArgumentException>(
                () => _usuarioService.CadastrarUsuarioAsync(dto)
            );

            Assert.Equal("Email já cadastrado.", excecao.Message);

            _repositoryMock.Verify(r => r.CadastrarUsuarioAsync(
                It.IsAny<Usuario>(),
                It.IsAny<CancellationToken>()),
                Times.Never
                );

            _repositoryMock.Verify(r => r.ExisteEmailAsync(
                dto.Email.Trim().ToLowerInvariant(), It.IsAny<CancellationToken>()),
                Times.Once
                );
        }

        [Fact]
        public async Task CadastrarUsuarioAsync_QuandoNomeInvalido_DeveLancarExcecao() {

            var dto = new UsuarioCadastroDto {
                Nome = "",
                Email = "maria@gmail.com",
                Senha = "Senha123!"
            };

            _repositoryMock
                .Setup(r => r.ExisteEmailAsync(
                    dto.Email.Trim().ToLowerInvariant(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _usuarioService.CadastrarUsuarioAsync(dto)
            );

            _repositoryMock.Verify(r => r.CadastrarUsuarioAsync(
                It.IsAny<Usuario>(),
                It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CadastrarUsuarioAsync_QuandoSenhaMenorQue8Caracteres_DeveLancarExcecao() {

            var usuarioDto = new UsuarioCadastroDto {
                Nome = "Maria",
                Email = "maria@gmail.com",
                Senha = "1234567"
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _usuarioService.CadastrarUsuarioAsync(usuarioDto)
            );

            Assert.Equal("A senha deve possuir pelo menos 8 caracteres.",
                exception.Message
            );

            _repositoryMock.Verify(repository => repository.ExisteEmailAsync(It.IsAny<string>()),
                Times.Never
            );

            _repositoryMock.Verify(repository => repository.CadastrarUsuarioAsync(It.IsAny<Usuario>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CadastrarUsuarioAsync_QuandoSenhaPossuiExatamente8Caracteres_DeveCadastrarUsuario() {

            var usuarioDto = new UsuarioCadastroDto {
                Nome = "Maria",
                Email = "maria@email.com",
                Senha = "12345678"
            };

            _repositoryMock.Setup(r => r.ExisteEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _repositoryMock.Setup(r => r.CadastrarUsuarioAsync(It.IsAny<Usuario>()))
                .Returns(Task.CompletedTask);

            var resultado = await _usuarioService.CadastrarUsuarioAsync(usuarioDto);

            Assert.NotNull(resultado);

            _repositoryMock.Verify(r => r.CadastrarUsuarioAsync(It.IsAny<Usuario>()),
                Times.Once);
        }

        [Fact]
        public async Task ObterUsuarioPorIdAsync_QuandoUsuarioExiste_DeveRetornarUsuario() {

            Usuario usuario = new Usuario(
                "Maria",
                "maria@gmail.com",
                "hashSenhaAntiga"
                );

            var idUsuario = 1;

            _repositoryMock.Setup(r => r.ObterUsuarioPorIdAsync(idUsuario))
                .ReturnsAsync(usuario);

            var resultado = await _usuarioService.ObterUsuarioPorIdAsync(idUsuario);

            Assert.NotNull(resultado);
            Assert.Equal(usuario.IdUsuario, resultado.IdUsuario);
            Assert.Equal(usuario.Email.Trim().ToLowerInvariant(), resultado.Email.Trim().ToLowerInvariant());

            _repositoryMock.Verify(r => r.ObterUsuarioPorIdAsync(
                idUsuario,
                It.IsAny<CancellationToken>()),
                Times.Once
                );
        }

        [Fact]
        public async Task ObterUsuariosAsync_QuandoExistemUsuarios_DeveRetornarLista() {

            Usuario usuarioExistente1 = new Usuario(
               "Maria",
               "maria@gmail.com",
               "hashSenhaAntiga"
           );

            Usuario usuarioExistente2 = new Usuario(
                "João",
                "joao@gmail.com",
                "hashSenhaAntiga"
            );

            _repositoryMock.Setup(r => r.ObterUsuariosAsync())
                .ReturnsAsync(new List<Usuario> {
                    usuarioExistente1,
                    usuarioExistente2
                });

            var resultado = await _usuarioService.ObterUsuariosAsync();

            Assert.NotEmpty(resultado);
            Assert.Equal(2, resultado.Count());
            Assert.Contains(usuarioExistente1, resultado);
            Assert.Contains(usuarioExistente2, resultado);

            _repositoryMock.Verify(r => r.ObterUsuariosAsync(
                It.IsAny<CancellationToken>()),
                Times.Once
                );
        }

        [Fact]
        public async Task LoginAsync_QuandoCredenciaisValidas_DeveRetornarToken() {

            var senha = "Senha123!";
            var email = "maria@gmail.com";

            var passwordHasher = new PasswordHasher<Usuario>();
            var senhaHash = passwordHasher.HashPassword(null!, senha);

            var usuario = new Usuario(
                "Maria",
                email,
                senhaHash
            );

            var loginDto = new LoginDto {
                Email = email,
                Senha = senha
            };

            _repositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(email,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _tokenServiceMock.Setup(t => t.GerarToken(usuario))
                .Returns("token-falso");

            var resultado = await _usuarioService.LoginAsync(loginDto);


            Assert.NotNull(resultado);
            Assert.Equal("token-falso", resultado.Token);

            _repositoryMock.Verify(r => r.ObterUsuarioPorEmailAsync(email,
                It.IsAny<CancellationToken>()),
                Times.Once
            );

            _tokenServiceMock.Verify(t => t.GerarToken(usuario), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_QuandoUsuarioNaoExiste_DeveLancarExcecao() {

            var loginDto = new LoginDto {
                Email = "naoexiste@gmail.com",
                Senha = "Senha123!"
            };

            _repositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(loginDto.Email.Trim().ToLowerInvariant(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            var excecao = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _usuarioService.LoginAsync(loginDto)
            );

            Assert.Equal("Email ou senha inválidos.", excecao.Message);

            _tokenServiceMock.Verify(
                t => t.GerarToken(It.IsAny<Usuario>()),
                Times.Never
            );
        }

        [Fact]
        public async Task LoginAsync_QuandoSenhaIncorreta_DeveLancarExcecao() {

            var senhaCorreta = "Senha123!";
            var email = "maria@gmail.com";

            var usuario = new Usuario(
                "Maria",
                email,
                new PasswordHasher<Usuario>().HashPassword(null!, senhaCorreta)
            );

            var loginDto = new LoginDto {
                Email = email,
                Senha = "SenhaErrada123!"
            };

            _repositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(email,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            var excecao = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _usuarioService.LoginAsync(loginDto)
            );

            Assert.Equal("Email ou senha inválidos.", excecao.Message);

            _tokenServiceMock.Verify(
                t => t.GerarToken(It.IsAny<Usuario>()),
                Times.Never
            );
        }

        [Fact]
        public async Task LoginAsync_QuandoEmailPossuiEspacosEMaiusculas_DeveNormalizarEmailERetornarToken() {

            var email = "maria@gmail.com";
            var senha = "Senha123!";

            var usuario = new Usuario(
                "Maria",
                email,
                new PasswordHasher<Usuario>().HashPassword(null!, senha)
            );

            var loginDto = new LoginDto {
                Email = " Maria@Gmail.com ",
                Senha = senha
            };

            _repositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(email,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _tokenServiceMock.Setup(t => t.GerarToken(usuario))
                .Returns("token-falso");

            var resultado = await _usuarioService.LoginAsync(loginDto);

            Assert.NotNull(resultado);
            Assert.Equal("token-falso", resultado.Token);

            _repositoryMock.Verify(r => r.ObterUsuarioPorEmailAsync(email,
                It.IsAny<CancellationToken>()),
                Times.Once
            );

            _tokenServiceMock.Verify(t => t.GerarToken(usuario), Times.Once);
        }

    }
}
