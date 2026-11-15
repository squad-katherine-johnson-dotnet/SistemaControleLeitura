using SistemaControleLeitura.Api.Entities;

namespace SistemaControleLeitura.Api.Repositories;

//define as operações de persistencia disponiveis para livros
public interface ILivroRepository
{
    // adiciona um livro e grava as alterações no banco
    Task AdicionarAsync(Livro livro);
}