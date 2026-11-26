using SistemaControleLeitura.Application.DTOs;

namespace SistemaControleLeitura.Application.Interfaces;

public interface IEstanteService
{
    Task<EstanteItemDto> AdicionarLivroAsync(
        int livroId,
        int usuarioId);

    Task<EstanteItemDto> AlterarStatusAsync(
        int leituraId,
        int usuarioId,
        AtualizarStatusLeituraDto dto);

    Task<EstanteItemDto> IniciarReleituraAsync(
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