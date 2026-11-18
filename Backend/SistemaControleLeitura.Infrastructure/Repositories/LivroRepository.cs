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
    }
}