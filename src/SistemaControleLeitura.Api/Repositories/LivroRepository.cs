using SistemaControleLeitura.Api.Data;
using SistemaControleLeitura.Api.Entities;
namespace SistemaControleLeitura.Api.Repositories;


// implementa o acesso aos livros usando o AppDbContext
public class LivroRepository : ILivroRepository
{
    // contexto usado para acessar o banco pelo EF core
    private readonly AppDbContext _context;

    //recebe o contexto por injeção de dependencia
    public LivroRepository(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }

    // adiciona o livro ao contexto e salva as alterações no banco
    public async Task AdicionarAsync(Livro livro)
    {
        await _context.Livros.AddAsync(livro);
        await _context.SaveChangesAsync();
    }
}