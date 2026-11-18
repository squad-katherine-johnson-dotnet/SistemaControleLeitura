using SistemaControleLeitura.Domain.Entities;

namespace SistemaControleLeitura.Application.Interfaces
{
    public interface ILivroRepository
    {
        Task CadastrarLivroAsync(Livro livro, CancellationToken cancellationToken = default); 
    }
}
