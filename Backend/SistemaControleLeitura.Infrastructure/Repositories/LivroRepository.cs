using Microsoft.EntityFrameworkCore;
using SistemaControleLeitura.Application.Interfaces;
using SistemaControleLeitura.Domain.Entities;
using SistemaControleLeitura.Infrastructure.Data;

namespace SistemaControleLeitura.Infrastructure.Repositories
{
    public class LivroRepository : ILivroRepository
    {
        private readonly AppDbContext _context;

        public LivroRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CadastrarLivroAsync(Livro livro, CancellationToken cancellationToken = default)
        {
            _context.Livros.Add(livro);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Livro>> ListarPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default)
        {
            // AsNoTracking: é só leitura, então o EF não precisa acompanhar as mudanças (mais leve).
            return await _context.Livros
                .AsNoTracking()
                .Where(l => l.UsuarioId == usuarioId)
                .OrderBy(l => l.Titulo)
                .ThenBy(l => l.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<Livro?> ObterPorIdAsync(int usuarioId, int livroId, CancellationToken cancellationToken = default)
        {
            // Sem AsNoTracking: o Service pode alterar este livro e depois chamar AtualizarAsync.
            // Filtrar também por UsuarioId garante que uma usuária nunca acessa o livro de outra (regra 9).
            return await _context.Livros
                .FirstOrDefaultAsync(l => l.Id == livroId && l.UsuarioId == usuarioId, cancellationToken);
        }

        public async Task AtualizarAsync(Livro livro, CancellationToken cancellationToken = default)
        {
            _context.Livros.Update(livro);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> LivroEstaNaEstanteAsync(int livroId, CancellationToken cancellationToken = default)
        {
            return await _context.ItensEstante
                .AnyAsync(i => i.LivroId == livroId, cancellationToken);
        }

        public async Task RemoverAsync(Livro livro, CancellationToken cancellationToken = default)
        {
            _context.Livros.Remove(livro);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
