using SistemaControleLeitura.Domain.Entities;

namespace SistemaControleLeitura.Domain.Interfaces
{
    // Contrato do repositório de livros: o Service diz o que precisa,
    // a Infrastructure (LivroRepository) cumpre com o banco.
    public interface ILivroRepository
    {
        Task CadastrarLivroAsync(Livro livro, CancellationToken cancellationToken = default);

        // Só os livros da usuária informada, em ordem alfabética de título.
        Task<List<Livro>> ListarPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default);

        // Devolve null se o livro não existe OU se pertence a outra usuária.
        Task<Livro?> ObterPorIdAsync(int usuarioId, int livroId, CancellationToken cancellationToken = default);

        // Salva as alterações feitas em um livro que já foi buscado com ObterPorIdAsync.
        Task AtualizarAsync(Livro livro, CancellationToken cancellationToken = default);

        // Regra 8: um livro que está na estante não pode ser excluído.
        Task<bool> LivroEstaNaEstanteAsync(int livroId, CancellationToken cancellationToken = default);

        Task RemoverAsync(Livro livro, CancellationToken cancellationToken = default);
    }
}
