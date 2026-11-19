using SistemaControleLeitura.Domain.Entities;
using SistemaControleLeitura.Application.DTOs;

namespace SistemaControleLeitura.Application.Interfaces;

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

    Task<List<EstanteItemDto>> ListarEstanteAsync(
        int usuarioId,
        string? status,
        int? ano);

    Task RemoverItemAsync(ItemEstante item);
    
    Task AdicionarNovaLeituraAsync(Leitura leitura);

    Task SalvarAlteracoesAsync();
}