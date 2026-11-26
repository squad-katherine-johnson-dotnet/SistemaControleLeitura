using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Domain.Entities;
using SistemaControleLeitura.Domain.Interfaces;
using SistemaControleLeitura.Application.Interfaces;

namespace SistemaControleLeitura.Application.Services;

public class LivroService : ILivroService
{
    private readonly ILivroRepository _livroRepository;

    public LivroService(ILivroRepository livroRepository)
    {
        _livroRepository = livroRepository;
    }

    public async Task<LivroRespostaDto> CadastrarLivroAsync(int usuarioId, LivroCadastroDto livroDto)
    {
        // 1. Cria a entidade. O construtor valida e lança ArgumentException se algo estiver inválido.
        var livro = new Livro(usuarioId, livroDto.Titulo, livroDto.Autor, livroDto.NumeroPaginas);

        // 2. Manda o repository gravar. Depois disto o banco já gerou o Id.
        await _livroRepository.CadastrarLivroAsync(livro);

        // 3 e 4. Converte para o DTO de resposta e devolve.
        return ParaRespostaDto(livro);
    }

    public async Task<List<LivroRespostaDto>> ListarLivrosAsync(int usuarioId)
    {
        var livros = await _livroRepository.ListarPorUsuarioAsync(usuarioId);

        return livros.Select(ParaRespostaDto).ToList();
    }

    public async Task<LivroRespostaDto?> ObterLivroPorIdAsync(int usuarioId, int livroId)
    {
        // O repository já filtra pela usuária: livro de outra pessoa volta como null.
        var livro = await _livroRepository.ObterPorIdAsync(usuarioId, livroId);

        return livro is null ? null : ParaRespostaDto(livro);
    }

    public async Task<LivroRespostaDto?> AtualizarLivroAsync(int usuarioId, int livroId, LivroCadastroDto livroDto)
    {
        var livro = await _livroRepository.ObterPorIdAsync(usuarioId, livroId);

        if (livro is null)
            return null;

        // A entidade valida e só então altera os campos.
        livro.Atualizar(livroDto.Titulo, livroDto.Autor, livroDto.NumeroPaginas);

        await _livroRepository.AtualizarAsync(livro);

        return ParaRespostaDto(livro);
    }

    public async Task<ResultadoExclusaoLivro> ExcluirLivroAsync(int usuarioId, int livroId)
    {
        var livro = await _livroRepository.ObterPorIdAsync(usuarioId, livroId);

        if (livro is null)
            return ResultadoExclusaoLivro.NaoEncontrado;

        // Regra 8: livro que está na estante não pode ser excluído.
        if (await _livroRepository.LivroEstaNaEstanteAsync(livro.Id))
            return ResultadoExclusaoLivro.NaEstante;

        await _livroRepository.RemoverAsync(livro);

        return ResultadoExclusaoLivro.Excluido;
    }

    // Um só lugar converte Livro em LivroRespostaDto, para os métodos acima não repetirem esse código.
    private static LivroRespostaDto ParaRespostaDto(Livro livro)
    {
        return new LivroRespostaDto
        {
            Id = livro.Id,
            Titulo = livro.Titulo,
            Autor = livro.Autor,
            NumeroPaginas = livro.NumeroPaginas
        };
    }
}
