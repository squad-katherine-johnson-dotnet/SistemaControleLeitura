using SistemaControleLeitura.Application.DTOs;


namespace SistemaControleLeitura.Application.Interfaces
{
    public interface ILivroService
    {
        Task<LivroRespostaDto> CadastrarLivroAsync(int usuarioId, LivroCadastroDto livroDto);

        Task<List<LivroRespostaDto>> ListarLivrosAsync(int usuarioId);

        // Devolve null quando o livro não existe ou não é da usuária (o Controller responde 404).
        Task<LivroRespostaDto?> ObterLivroPorIdAsync(int usuarioId, int livroId);

        // Devolve null quando o livro não existe ou não é da usuária (o Controller responde 404).
        Task<LivroRespostaDto?> AtualizarLivroAsync(int usuarioId, int livroId, LivroCadastroDto livroDto);

        Task<ResultadoExclusaoLivro> ExcluirLivroAsync(int usuarioId, int livroId);
    }
}
