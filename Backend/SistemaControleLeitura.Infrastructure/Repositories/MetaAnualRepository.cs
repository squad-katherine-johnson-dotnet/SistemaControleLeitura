using Microsoft.EntityFrameworkCore;
using SistemaControleLeitura.Domain.Entities;
using SistemaControleLeitura.Domain.Enums;
using SistemaControleLeitura.Domain.Interfaces;
using SistemaControleLeitura.Infrastructure.Data;

namespace SistemaControleLeitura.Infrastructure.Repositories;

public class MetaAnualRepository : IMetaAnualRepository
{
    private readonly AppDbContext _context;

    public MetaAnualRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MetaAnual?> ObterPorUsuarioEAnoAsync(int usuarioId, int ano)
    {
        return await _context.MetasAnuais
            .FirstOrDefaultAsync(m => m.UsuarioId == usuarioId && m.Ano == ano);
    }

    public async Task AdicionarAsync(MetaAnual metaAnual)
    {
        _context.MetasAnuais.Add(metaAnual);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(MetaAnual metaAnual)
    {
        _context.MetasAnuais.Update(metaAnual);
        await _context.SaveChangesAsync();
    }

    public async Task<int> ContarLivrosLidosNoAnoAsync(int usuarioId, int ano)
    {
        return await (
            from leitura in _context.Leituras
            join item in _context.ItensEstante on leitura.ItemEstanteId equals item.Id
            join livro in _context.Livros on item.LivroId equals livro.Id
            where livro.UsuarioId == usuarioId
                  && leitura.Status == StatusLeitura.Lido
                  && leitura.DataTermino.HasValue
                  && leitura.DataTermino.Value.Year == ano
            select leitura.Id
        ).CountAsync();
    }
}