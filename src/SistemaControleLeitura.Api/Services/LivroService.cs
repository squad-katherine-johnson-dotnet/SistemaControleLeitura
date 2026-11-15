using SistemaControleLeitura.Api.Repositories;
using SistemaControleLeitura.Api.Dtos.Livros;
using SistemaControleLeitura.Api.Entities;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;

namespace SistemaControleLeitura.Api.Services;

// coordena o cadastro: transforma os dados recebidos em Livro e pede pra salvar
public class LivroService
{
    // repositorio responsavel por acessar os dados do livro
    private readonly ILivroRepository _livroRepository;
    
    // recebe o repositprio por injeçao de dependencia
    public LivroService(ILivroRepository livroRepository)
    {
        _livroRepository = livroRepository;
    }

    // cria e  salva um livro associado ao usuario informado
    public async Task<Livro> CriarLivroAsync(CriarLivroRequest criarLivroRequest, int usuarioId)
    {
        // monta a entidade com os dados recebidos e a usuaria dona do livro
        Livro livro = new Livro
        {
            UsuarioId = usuarioId,
            Autor = criarLivroRequest.Autor,
            Titulo = criarLivroRequest.Titulo,
            NumeroPaginas = criarLivroRequest.NumeroPaginas,
        };
        

        //pede ao repositorio para gravar o livro no banco
        await _livroRepository.AdicionarAsync(livro);
        // devolve o livro salvo incluindo o id para o controller preparar a resposta
        return livro;
    }


}