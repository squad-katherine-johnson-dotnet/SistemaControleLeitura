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
    }
}