using SistemaControleLeitura.Application.DTOs;


namespace SistemaControleLeitura.Application.Interfaces
{
    public interface ILivroService
    {
        Task<LivroRespostaDto> CadastrarLivroAsync(int usuarioId, LivroCadastroDto livroDto);


    }
}