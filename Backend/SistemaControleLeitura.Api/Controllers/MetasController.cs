using Microsoft.AspNetCore.Mvc;
using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Interfaces;

namespace SistemaControleLeitura.Api.Controllers
{
    [ApiController]
    [Route("api/metas")]
    public class MetasController : BaseController
    {
        private readonly IMetaAnualService _metaAnualService;

        public MetasController(IMetaAnualService metaAnualService)
        {
            _metaAnualService = metaAnualService;
        }

        [HttpPut("{ano}")]
        public async Task<IActionResult> DefinirMetaAnualAsync(int ano, [FromBody] DefinirMetaAnualDto metaDto)
        {
            try
            {
                var metaDefinida = await _metaAnualService.DefinirMetaAnualAsync(UsuarioId, ano, metaDto);

                var respostaPadrao = new RespostaDto<MetaAnualRespostaDto>
                {
                    Sucesso = true,
                    Mensagem = "Meta anual definida com sucesso.",
                    Dados = metaDefinida
                };

                return Ok(respostaPadrao);
            }
            catch (ArgumentException ex)
            {
                var respostaErro = new RespostaDto<object>
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                };

                return BadRequest(respostaErro);
            }
        }

        [HttpGet("{ano}/progresso")]
        public async Task<IActionResult> ConsultarProgressoAsync(int ano)
        {
            try
            {
                var progresso = await _metaAnualService.ConsultarProgressoAsync(UsuarioId, ano);

                var respostaPadrao = new RespostaDto<ProgressoMetaAnualDto>
                {
                    Sucesso = true,
                    Mensagem = "Progresso da meta anual consultado com sucesso.",
                    Dados = progresso
                };

                return Ok(respostaPadrao);
            }
            catch (UnauthorizedAccessException ex)
            {
                var respostaErro = new RespostaDto<object>
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                };

                return Unauthorized(respostaErro);
            }
            catch (ArgumentException ex)
            {
                var respostaErro = new RespostaDto<object>
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                };

                return BadRequest(respostaErro);
            }
            catch (KeyNotFoundException ex)
            {
                var respostaErro = new RespostaDto<object>
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                };

                return NotFound(respostaErro);
            }
        }
    }
}