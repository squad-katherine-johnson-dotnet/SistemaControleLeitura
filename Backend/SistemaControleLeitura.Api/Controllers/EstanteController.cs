using Microsoft.AspNetCore.Mvc;
using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Interfaces;

namespace SistemaControleLeitura.Api.Controllers;

[ApiController]
[Route("api")]
public class EstanteController : BaseController
{
    private readonly IEstanteService _service;

    public EstanteController(IEstanteService service)
    {
        _service = service;
    }

    [HttpPost("estante")]
    public async Task<IActionResult> AdicionarLivroAsync(
        [FromBody] AdicionarEstanteDto dto)
    {
        try
        {
            await _service.AdicionarLivroAsync(
                dto.LivroId, UsuarioId);

            return StatusCode(201, new RespostaDto<object>
            {
                Sucesso = true,
                Mensagem = "Livro adicionado à estante com sucesso.",
                Dados = null
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(Erro(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Erro(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(Erro(ex.Message));
        }
    }

    [HttpPatch("leituras/{id}/status")]
    public async Task<IActionResult> AlterarStatusAsync(
        int id,
        [FromBody] AtualizarStatusLeituraDto dto)
    {
        try
        {
            await _service.AlterarStatusAsync(
                id, UsuarioId, dto);

            return Ok(new RespostaDto<object>
            {
                Sucesso = true,
                Mensagem = "Status da leitura atualizado com sucesso.",
                Dados = null
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(Erro(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Erro(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(Erro(ex.Message));
        }
    }

    [HttpPost("estante/{id}/leituras")]
    public async Task<IActionResult> IniciarReleituraAsync(int id)
    {
        try
        {
            await _service.IniciarReleituraAsync(
                id, UsuarioId);

            return StatusCode(201, new RespostaDto<object>
            {
                Sucesso = true,
                Mensagem = "Nova leitura iniciada com sucesso.",
                Dados = null
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Erro(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(Erro(ex.Message));
        }
    }

    [HttpGet("estante")]
    public async Task<IActionResult> ListarEstanteAsync(
        [FromQuery] string? status,
        [FromQuery] int? ano)
    {
        try
        {
            var livros = await _service.ListarEstanteAsync(
                UsuarioId, status, ano);

            return Ok(new RespostaDto<List<EstanteItemDto>>
            {
                Sucesso = true,
                Mensagem = livros.Count > 0
                    ? "Estante consultada com sucesso."
                    : "Nenhum livro encontrado.",
                Dados = livros
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(Erro(ex.Message));
        }
    }

    [HttpDelete("estante/{id}")]
    public async Task<IActionResult> RemoverLivroAsync(int id)
    {
        try
        {
            await _service.RemoverLivroAsync(id, UsuarioId);

            return Ok(new RespostaDto<object>
            {
                Sucesso = true,
                Mensagem = "Livro retirado da estante com sucesso.",
                Dados = null
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Erro(ex.Message));
        }
    }

    private static RespostaDto<object> Erro(string mensagem)
    {
        return new RespostaDto<object>
        {
            Sucesso = false,
            Mensagem = mensagem,
            Dados = null
        };
    }
}