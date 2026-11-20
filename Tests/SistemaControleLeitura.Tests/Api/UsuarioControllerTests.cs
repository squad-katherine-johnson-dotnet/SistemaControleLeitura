using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SistemaControleLeitura.Api.Controllers;
using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Interfaces;
using SistemaControleLeitura.Domain.Entities;

namespace SistemaControleLeitura.Tests.Api {
    public class UsuarioControllerTests {

        private readonly Mock<IUsuarioService> _serviceMock;
        private readonly UsuarioController _usuarioController;

        public UsuarioControllerTests() {
            _serviceMock = new Mock<IUsuarioService>();
            _usuarioController = new UsuarioController(_serviceMock.Object);
        }

        [Fact]
        public async Task CadastrarUsuarioAsync_QuandoDadosValidos_DeveRetornarCreated() {

            var dto = new UsuarioCadastroDto {
                Nome = "Maria",
                Email = "maria@gmail.com",
                Senha = "Senha123!"
            };

            var usuarioResposta = new UsuarioRespostaDto {
                Id = 1,
                Nome = dto.Nome,
                Email = dto.Email
            };

            _serviceMock
                .Setup(s => s.CadastrarUsuarioAsync(dto))
                .ReturnsAsync(usuarioResposta);

            var resultado = await _usuarioController.CadastrarUsuarioAsync(dto);

            var createdResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(201, createdResult.StatusCode);

            var resposta = Assert.IsType<RespostaDto<UsuarioRespostaDto>>(
                createdResult.Value
            );

            Assert.True(resposta.Sucesso);
            Assert.Equal("Usuário cadastrado com sucesso.", resposta.Mensagem);

            Assert.NotNull(resposta.Dados);
            Assert.Equal(usuarioResposta.Id, resposta.Dados.Id);
            Assert.Equal(usuarioResposta.Nome, resposta.Dados.Nome);
            Assert.Equal(usuarioResposta.Email, resposta.Dados.Email);

            _serviceMock.Verify(
                s => s.CadastrarUsuarioAsync(dto),
                Times.Once
            );
        }

        [Fact]
        public async Task CadastrarUsuarioAsync_QuandoEmailJaCadastrado_DeveRetornarConflict() {

            var dto = new UsuarioCadastroDto {
                Nome = "Maria",
                Email = "maria@gmail.com",
                Senha = "Senha123!"
            };

            _serviceMock
                .Setup(s => s.CadastrarUsuarioAsync(dto))
                .ThrowsAsync(new ArgumentException("Email já cadastrado."));

            var resultado = await _usuarioController.CadastrarUsuarioAsync(dto);

            var conflictResult = Assert.IsType<ConflictObjectResult>(resultado);

            var resposta = Assert.IsType<RespostaDto<object>>(conflictResult.Value);

            Assert.False(resposta.Sucesso);
            Assert.Equal("Email já cadastrado.", resposta.Mensagem);
            Assert.Null(resposta.Dados);

            _serviceMock.Verify(s => s.CadastrarUsuarioAsync(dto),
                Times.Once
                );
        }

        [Fact]
        public async Task CadastrarUsuarioAsync_QuandoDadosInvalidos_DeveRetornarBadRequest() {

            var dto = new UsuarioCadastroDto {
                Nome = "",
                Email = "maria@gmail.com",
                Senha = "Senha123!"
            };

            _serviceMock
                .Setup(s => s.CadastrarUsuarioAsync(dto))
                .ThrowsAsync(new ArgumentException("Nome informado é inválido."));

            var resultado = await _usuarioController.CadastrarUsuarioAsync(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);

            var resposta = Assert.IsType<RespostaDto<object>>(badRequest.Value);

            Assert.False(resposta.Sucesso);
            Assert.Equal("Nome informado é inválido.", resposta.Mensagem);
            Assert.Null(resposta.Dados);

            _serviceMock.Verify(s => s.CadastrarUsuarioAsync(dto),
                Times.Once
                );
        }

        [Fact]
        public async Task ObterUsuariosAsync_QuandoExistemUsuarios_DeveRetornarOk() {

            var usuarios = new List<Usuario> {
                new Usuario("Maria", "maria@gmail.com", "hashSenha123"),
                new Usuario("João", "joao@gmail.com", "hashSenha456")
            };

            _serviceMock
                .Setup(s => s.ObterUsuariosAsync())
                .ReturnsAsync(usuarios);

            var resultado = await _usuarioController.ObterUsuariosAsync();

            var okResult = Assert.IsType<OkObjectResult>(resultado);

            var resposta = Assert.IsType<RespostaDto<List<UsuarioRespostaDto>>>(
                okResult.Value
            );

            Assert.True(resposta.Sucesso);
            Assert.Equal("Usuários encontrados.", resposta.Mensagem);

            Assert.NotNull(resposta.Dados);
            Assert.Equal(2, resposta.Dados.Count);

            Assert.Equal(usuarios[0].IdUsuario, resposta.Dados[0].Id);
            Assert.Equal(usuarios[0].Nome, resposta.Dados[0].Nome);
            Assert.Equal(usuarios[0].Email, resposta.Dados[0].Email);

            Assert.Equal(usuarios[1].IdUsuario, resposta.Dados[1].Id);
            Assert.Equal(usuarios[1].Nome, resposta.Dados[1].Nome);
            Assert.Equal(usuarios[1].Email, resposta.Dados[1].Email);

            _serviceMock.Verify(
                s => s.ObterUsuariosAsync(),
                Times.Once
            );
        }

        [Fact]
        public async Task ObterUsuariosAsync_QuandoNaoExistemUsuarios_DeveRetornarOkComListaVazia() {

            var usuarios = new List<Usuario>();

            _serviceMock
                .Setup(s => s.ObterUsuariosAsync())
                .ReturnsAsync(usuarios);

            var resultado = await _usuarioController.ObterUsuariosAsync();

            var okResult = Assert.IsType<OkObjectResult>(resultado);

            var resposta = Assert.IsType<RespostaDto<List<UsuarioRespostaDto>>>(
                okResult.Value
            );

            Assert.True(resposta.Sucesso);
            Assert.Equal("Nenhum usuário cadastrado.", resposta.Mensagem);

            Assert.NotNull(resposta.Dados);
            Assert.Empty(resposta.Dados);

            _serviceMock.Verify(
                s => s.ObterUsuariosAsync(),
                Times.Once
            );
        }

        [Fact]
        public async Task AutenticarUsuarioAsync_QuandoCredenciaisValidas_DeveRetornarOk() {

            var loginDto = new LoginDto {
                Email = "maria@gmail.com",
                Senha = "Senha123"
            };

            var loginResposta = new LoginRespostaDto {
                Token = "token-falso"
            };

            _serviceMock.Setup(service => service.LoginAsync(loginDto))
                .ReturnsAsync(loginResposta);

            var resultado = await _usuarioController.AutenticarUsuarioAsync(loginDto);

            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var resposta = Assert.IsType<RespostaDto<LoginRespostaDto>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.True(resposta.Sucesso);
            Assert.Equal("Login realizado com sucesso.", resposta.Mensagem);

            Assert.NotNull(resposta.Dados);
            Assert.Equal("token-falso", resposta.Dados.Token);

            _serviceMock.Verify(service => service.LoginAsync(loginDto),
                Times.Once
            );
        }

        [Fact]
        public async Task AutenticarUsuarioAsync_QuandoCredenciaisInvalidas_DeveRetornarUnauthorized() {

            var loginDto = new LoginDto {
                Email = "maria@gmail.com",
                Senha = "SenhaErrada"
            };

            _serviceMock.Setup(service => service.LoginAsync(loginDto))
                .ThrowsAsync(new UnauthorizedAccessException("Email ou senha inválidos."));

            var resultado = await _usuarioController.AutenticarUsuarioAsync(loginDto);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(resultado);
            var resposta = Assert.IsType<RespostaDto<object>>(unauthorizedResult.Value);

            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.False(resposta.Sucesso);
            Assert.Equal("Email ou senha inválidos.", resposta.Mensagem);
            Assert.Null(resposta.Dados);

            _serviceMock.Verify(service => service.LoginAsync(loginDto),
                Times.Once
            );
        }
    }
}
