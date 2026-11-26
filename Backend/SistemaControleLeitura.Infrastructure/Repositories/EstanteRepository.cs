using Microsoft.EntityFrameworkCore;
using SistemaControleLeitura.Domain.Entities;
using SistemaControleLeitura.Domain.Enums;
using SistemaControleLeitura.Domain.Interfaces;
using SistemaControleLeitura.Infrastructure.Data;

namespace SistemaControleLeitura.Infrastructure.Repositories;

public class EstanteRepository : IEstanteRepository {
    private readonly AppDbContext _context;

    public EstanteRepository(AppDbContext context) {
        _context = context;
    }

    public async Task<Livro?> ObterLivroDoUsuarioAsync(int livroId, int usuarioId) {
        return await _context.Livros
            .FirstOrDefaultAsync(l =>
                l.Id == livroId &&
                l.UsuarioId == usuarioId);
    }

    public async Task<bool> LivroJaEstaNaEstanteAsync(int livroId) {
        return await _context.ItensEstante
            .AnyAsync(i => i.LivroId == livroId);
    }

    public async Task AdicionarItemComLeituraAsync(ItemEstante item, Leitura leitura) {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try {
            _context.ItensEstante.Add(item);

            await _context.SaveChangesAsync();

            leitura.ItemEstanteId = item.Id;

            _context.Leituras.Add(leitura);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task AdicionarNovaLeituraAsync(Leitura leitura) {
        _context.Leituras.Add(leitura);

        await _context.SaveChangesAsync();
    }
    public async Task<(ItemEstante? Item, Livro? Livro, Leitura? UltimaLeitura)>
        ObterItemComUltimaLeituraAsync(int itemId, int usuarioId) {
        var item = await _context.ItensEstante
            .FirstOrDefaultAsync(i => i.Id == itemId);

        if (item == null)
            return (null, null, null);

        var livro = await _context.Livros
            .FirstOrDefaultAsync(l =>
                l.Id == item.LivroId &&
                l.UsuarioId == usuarioId);

        if (livro == null)
            return (null, null, null);

        var ultimaLeitura = await _context.Leituras
            .Where(l => l.ItemEstanteId == itemId)
            .OrderByDescending(l => l.Id)
            .FirstOrDefaultAsync();

        return (item, livro, ultimaLeitura);
    }

    public async Task<Leitura?> ObterLeituraDoUsuarioAsync(int leituraId, int usuarioId) {
        var leitura = await (
            from l in _context.Leituras
            join i in _context.ItensEstante
                on l.ItemEstanteId equals i.Id
            join livro in _context.Livros
                on i.LivroId equals livro.Id
            where l.Id == leituraId &&
                  livro.UsuarioId == usuarioId
            select l
        ).FirstOrDefaultAsync();

        return leitura;
    }

    public async Task<List<(ItemEstante Item, Livro Livro, Leitura UltimaLeitura)>>
        ListarEstanteAsync(int usuarioId, StatusLeitura? status, int? ano) {

        var itens = await (
            from item in _context.ItensEstante.AsNoTracking()
            join livro in _context.Livros.AsNoTracking()
                on item.LivroId equals livro.Id
            where livro.UsuarioId == usuarioId
            select new {
                Item = item,
                Livro = livro
            }
        ).ToListAsync();

        var ids = itens.Select(x => x.Item.Id).ToList();

        var leituras = await _context.Leituras
            .AsNoTracking()
            .Where(l => ids.Contains(l.ItemEstanteId))
            .ToListAsync();

        var resultado = new List<(
            ItemEstante Item,
            Livro Livro,
            Leitura UltimaLeitura)>();

        foreach (var registro in itens) {
            var ultimaLeitura = leituras
                .Where(l => l.ItemEstanteId == registro.Item.Id)
                .OrderByDescending(l => l.Id)
                .FirstOrDefault();

            if (ultimaLeitura == null)
                continue;

            if (status.HasValue && ultimaLeitura.Status != status.Value)
                continue;

            if (ano.HasValue &&
                (!ultimaLeitura.DataTermino.HasValue ||
                 ultimaLeitura.DataTermino.Value.Year != ano.Value))
                continue;


            resultado.Add((
                registro.Item,
                registro.Livro,
                ultimaLeitura));
        }

        return resultado;
    }

    public async Task RemoverItemAsync(ItemEstante item) {
        _context.ItensEstante.Remove(item);

        await _context.SaveChangesAsync();
    }

    public async Task SalvarAlteracoesAsync() {
        await _context.SaveChangesAsync();
    }
}