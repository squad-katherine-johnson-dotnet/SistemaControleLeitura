using Microsoft.AspNetCore.Mvc;
using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Interfaces;
using SistemaControleLeitura.Domain.Entities;

namespace SistemaControleLeitura.Api.Controllers {

    [ApiController]
    [Route("api/usuarios")]
    public class UsuarioController : BaseController {

        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService) {
            _usuarioService = usuarioService;
        }

        [HttpPost]
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

                if (ex.Message == "E-mail já cadastrado.")
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
    }
}
