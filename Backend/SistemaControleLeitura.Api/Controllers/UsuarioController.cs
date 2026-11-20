using Microsoft.AspNetCore.Mvc;
using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SistemaControleLeitura.Api.Controllers {

    [ApiController]
    [Route("api/usuarios")]
    [Authorize]
    public class UsuarioController : BaseController {

        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService) {
            _usuarioService = usuarioService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CadastrarUsuarioAsync([FromBody] UsuarioCadastroDto usuarioDto) {

            try {

                var usuarioCriado = await _usuarioService.CadastrarUsuarioAsync(usuarioDto);

                var respostaUsuario = new UsuarioRespostaDto {
                    Id = usuarioCriado.Id,
                    Nome = usuarioCriado.Nome,
                    Email = usuarioCriado.Email
                };


                var respostaPadrao = new RespostaDto<UsuarioRespostaDto> {
                    Sucesso = true,
                    Mensagem = "Usuário cadastrado com sucesso.",
                    Dados = respostaUsuario
                };

                return StatusCode(201, respostaPadrao);
            }
            catch (ArgumentException ex) {

                var resposta = new RespostaDto<object> {
                    Sucesso = false,
                    Mensagem = ex.Message
                };

                if (ex.Message == "Email já cadastrado.")
                    return Conflict(resposta);

                return BadRequest(resposta);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterUsuariosAsync() {

            var usuarios = await _usuarioService.ObterUsuariosAsync();

            if (!usuarios.Any()) {
                var respostaVazia = new RespostaDto<List<UsuarioRespostaDto>> {
                    Sucesso = true,
                    Mensagem = "Nenhum usuário cadastrado.",
                    Dados = new List<UsuarioRespostaDto>()
                };

                return Ok(respostaVazia);
            }

            var respostaUsuario = usuarios.Select(usuario => new UsuarioRespostaDto {
                Id = usuario.IdUsuario,
                Nome = usuario.Nome,
                Email = usuario.Email,
            }).ToList();

            var respostaPadrao = new RespostaDto<List<UsuarioRespostaDto>> {
                Sucesso = true,
                Mensagem = "Usuários encontrados.",
                Dados = respostaUsuario
            };

            return Ok(respostaPadrao);
        }

        [HttpPost("/api/autenticacao/login")]
        [AllowAnonymous]
        public async Task<IActionResult> AutenticarUsuarioAsync([FromBody] LoginDto loginDto) {

            try {
                var login = await _usuarioService.LoginAsync(loginDto);

                var resposta = new RespostaDto<LoginRespostaDto>() {
                    Sucesso = true,
                    Mensagem = "Login realizado com sucesso.",
                    Dados = login
                };

                return Ok(resposta);
            }
            catch (UnauthorizedAccessException ex) {

                var resposta = new RespostaDto<object> {
                    Sucesso = false,
                    Mensagem = ex.Message
                };

                return Unauthorized(resposta);
            }
        }
    }
}
