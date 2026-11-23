using Microsoft.AspNetCore.Mvc;
using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Interfaces;

namespace SistemaControleLeitura.Api.Controllers
{
    [ApiController]
    [Route("api/livros")]
    public class LivrosController : BaseController
    {
        private readonly ILivroService _livroService;

        public LivrosController(ILivroService livroService)
        {
            _livroService = livroService;
        }

        // POST /api/livros
        [HttpPost]
        public async Task<IActionResult> CadastrarLivroAsync([FromBody] LivroCadastroDto livroDto)
        {
            try
            {
                var livroCadastrado = await _livroService.CadastrarLivroAsync(UsuarioId, livroDto);

                var resposta = new RespostaDto<LivroRespostaDto>
                {
                    Sucesso = true,
                    Mensagem = "Livro cadastrado com sucesso.",
                    Dados = livroCadastrado
                };

                return StatusCode(201, resposta);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(Erro(ex.Message));
            }
        }

        // GET /api/livros
        [HttpGet]
        public async Task<IActionResult> ListarLivrosAsync()
        {
            var livros = await _livroService.ListarLivrosAsync(UsuarioId);

            var resposta = new RespostaDto<List<LivroRespostaDto>>
            {
                Sucesso = true,
                Mensagem = livros.Count == 0 ? "Nenhum livro cadastrado." : "Livros encontrados.",
                Dados = livros
            };

            return Ok(resposta);
        }

        // GET /api/livros/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterLivroPorIdAsync(int id)
        {
            var livro = await _livroService.ObterLivroPorIdAsync(UsuarioId, id);

            if (livro is null)
                return NotFound(Erro("Livro não encontrado."));

            var resposta = new RespostaDto<LivroRespostaDto>
            {
                Sucesso = true,
                Mensagem = "Livro encontrado.",
                Dados = livro
            };

            return Ok(resposta);
        }

        // PUT /api/livros/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> AtualizarLivroAsync(int id, [FromBody] LivroCadastroDto livroDto)
        {
            try
            {
                var livroAtualizado = await _livroService.AtualizarLivroAsync(UsuarioId, id, livroDto);

                if (livroAtualizado is null)
                    return NotFound(Erro("Livro não encontrado."));

                var resposta = new RespostaDto<LivroRespostaDto>
                {
                    Sucesso = true,
                    Mensagem = "Livro atualizado com sucesso.",
                    Dados = livroAtualizado
                };

                return Ok(resposta);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(Erro(ex.Message));
            }
        }

        // DELETE /api/livros/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> ExcluirLivroAsync(int id)
        {
            var resultado = await _livroService.ExcluirLivroAsync(UsuarioId, id);

            switch (resultado)
            {
                case ResultadoExclusaoLivro.NaoEncontrado:
                    return NotFound(Erro("Livro não encontrado."));

                case ResultadoExclusaoLivro.NaEstante:
                    return Conflict(Erro("O livro está na estante e não pode ser excluído."));

                default:
                    return Ok(new RespostaDto<object>
                    {
                        Sucesso = true,
                        Mensagem = "Livro excluído com sucesso."
                    });
            }
        }

        // Monta a resposta de erro no envelope padrão do projeto.
        private static RespostaDto<object> Erro(string mensagem)
        {
            return new RespostaDto<object>
            {
                Sucesso = false,
                Mensagem = mensagem
            };
        }
    }
}
