using SistemaControleLeitura.Application.DTOs;

namespace SistemaControleLeitura.Application.Interfaces;

public interface IEstanteService
{
    Task AdicionarLivroAsync(
        int livroId,
        int usuarioId);

    Task AlterarStatusAsync(
        int leituraId,
        int usuarioId,
        AtualizarStatusLeituraDto dto);

    Task IniciarReleituraAsync(
        int itemId,
        int usuarioId);

    Task<List<EstanteItemDto>> ListarEstanteAsync(
        int usuarioId,
        string? status,
        int? ano);

    Task RemoverLivroAsync(
        int itemId,
        int usuarioId);
}