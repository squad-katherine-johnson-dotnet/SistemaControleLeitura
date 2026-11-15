using Microsoft.AspNetCore.Mvc;
using SistemaControleLeitura.Api.Services;
using SistemaControleLeitura.Api.Dtos.Livros;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SistemaControleLeitura.Api.Entities;

namespace SistemaControleLeitura.Api.Controllers;

// ativa comportamentos de API com validação automatica e [route] define a rota base /api/livros
[ApiController]
[Route("api/[controller]")]
[Authorize] // exige uma usuaria autenticada
public class LivrosController : ControllerBase
{
    // serviço usado pelo controller nas operações com livros
    private readonly LivroService _livroService;

    //recebe o serviço por injeção de dependencia
    public LivrosController(LivroService livroService)
    {
        _livroService = livroService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarLivroRequest criarLivroRequest)
    {
        // le do token o ID da usuaria autenticada
        string? idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // impede de continuar se o token não trouxer um id numerico
        if (!int.TryParse(idUsuarioClaim, out int usuarioId))
        {
            return Unauthorized(new
            {
                Sucesso = false,
                Mensagem = "Token inválido ou sem autenticação da usuária.",
                Dados = (object?)null
            });
        }

        // delega ao serviço a criação e gravação do livro
        Livro livro = await _livroService.CriarLivroAsync(criarLivroRequest, usuarioId);

        // retorna Http 201 (created) com os dados do livro criado
        return Created($"/api/livros/{livro.Id}", new
        {
            Sucesso = true,
            Mensagem = "Livro cadastrado com sucesso.",
            Dados = new
            {
                livro.Id,
                livro.Titulo,
                livro.Autor,
                livro.NumeroPaginas
            }
        });
    }
}