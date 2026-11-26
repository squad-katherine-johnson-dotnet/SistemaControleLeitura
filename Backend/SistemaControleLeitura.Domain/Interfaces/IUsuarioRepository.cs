using SistemaControleLeitura.Domain.Entities;

namespace SistemaControleLeitura.Domain.Interfaces {
    public interface IUsuarioRepository {

        Task CadastrarUsuarioAsync(Usuario usuario, CancellationToken cancellationToken = default);

        Task<Usuario?> ObterUsuarioPorIdAsync(int idUsuario, CancellationToken cancellationToken = default);

        Task<Usuario?> ObterUsuarioPorEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<IEnumerable<Usuario>> ObterUsuariosAsync(CancellationToken cancellationToken = default);
    }
}
