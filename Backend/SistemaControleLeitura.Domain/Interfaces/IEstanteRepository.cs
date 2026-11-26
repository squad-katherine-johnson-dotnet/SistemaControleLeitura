using SistemaControleLeitura.Domain.Entities;
using SistemaControleLeitura.Domain.Enums;

namespace SistemaControleLeitura.Domain.Interfaces;

public interface IEstanteRepository
{
    Task<Livro?> ObterLivroDoUsuarioAsync(
        int livroId,
        int usuarioId);

    Task<bool> LivroJaEstaNaEstanteAsync(
        int livroId);

    Task AdicionarItemComLeituraAsync(
        ItemEstante item,
        Leitura leitura);

    Task<(ItemEstante? Item, Livro? Livro, Leitura? UltimaLeitura)>
        ObterItemComUltimaLeituraAsync(
            int itemId,
            int usuarioId);

    Task<Leitura?> ObterLeituraDoUsuarioAsync(
        int leituraId,
        int usuarioId);

    Task<List<(ItemEstante Item, Livro Livro, Leitura UltimaLeitura)>>
        ListarEstanteAsync(int usuarioId, StatusLeitura? status, int? ano);

    Task RemoverItemAsync(ItemEstante item);
    
    Task AdicionarNovaLeituraAsync(Leitura leitura);

    Task SalvarAlteracoesAsync();
}