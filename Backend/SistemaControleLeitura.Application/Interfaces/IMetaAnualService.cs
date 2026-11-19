using SistemaControleLeitura.Application.DTOs;

namespace SistemaControleLeitura.Application.Interfaces
{
    public interface IMetaAnualService
    {
        Task<MetaAnualRespostaDto> DefinirMetaAnualAsync(int usuarioId, int ano, DefinirMetaAnualDto metaDto);
    }
}