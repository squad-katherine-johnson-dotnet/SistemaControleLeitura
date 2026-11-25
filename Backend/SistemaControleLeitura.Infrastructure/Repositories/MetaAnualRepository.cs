using Microsoft.EntityFrameworkCore;
using SistemaControleLeitura.Domain.Entities;
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
        await _context.MetasAnuais.AddAsync(metaAnual);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(MetaAnual metaAnual)
    {
        _context.MetasAnuais.Update(metaAnual);
        await _context.SaveChangesAsync();
    }
}