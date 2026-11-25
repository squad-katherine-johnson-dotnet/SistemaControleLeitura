using SistemaControleLeitura.Domain.Entities;

namespace SistemaControleLeitura.Domain.Interfaces;

public interface IMetaAnualRepository
{
    Task<MetaAnual?> ObterPorUsuarioEAnoAsync(int usuarioId, int ano);
    Task AdicionarAsync(MetaAnual metaAnual);
    Task AtualizarAsync(MetaAnual metaAnual);
    Task<int> ContarLivrosLidosNoAnoAsync(int usuarioId, int ano);
}