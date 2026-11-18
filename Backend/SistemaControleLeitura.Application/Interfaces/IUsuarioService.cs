using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Domain.Entities;

namespace SistemaControleLeitura.Application.Interfaces {
    public interface IUsuarioService {

        Task<UsuarioRespostaDto> CadastrarUsuarioAsync(UsuarioCadastroDto usuarioDto);

        Task<Usuario?> ObterUsuarioPorIdAsync(int idUsuario);

        Task<Usuario?> ObterUsuarioPorEmailAsync(string email);

        Task<IEnumerable<Usuario>> ObterUsuariosAsync();

    }
}
