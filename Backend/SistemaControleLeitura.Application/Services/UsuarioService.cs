using Microsoft.AspNetCore.Identity;
using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Interfaces;
using SistemaControleLeitura.Domain.Entities;

namespace SistemaControleLeitura.Application.Services {
    public class UsuarioService : IUsuarioService {

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public UsuarioService(IUsuarioRepository usuarioRepository) {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        public async Task<UsuarioRespostaDto> CadastrarUsuarioAsync(UsuarioCadastroDto usuarioDto) {

            var email = usuarioDto.Email.Trim().ToLowerInvariant();

            if (await _usuarioRepository.ExisteEmailAsync(email))
                throw new ArgumentException("E-mail já cadastrado.");

            var senhaHash = _passwordHasher.HashPassword(null!, usuarioDto.Senha);

            var usuario = new Usuario(
                usuarioDto.Nome,
                email,
                senhaHash
            );

            await _usuarioRepository.CadastrarUsuarioAsync(usuario);

            var resposta = new UsuarioRespostaDto {
                Id = usuario.IdUsuario,
                Nome = usuario.Nome,
                Email = email,
            };

            return resposta;
        }

        public async Task<Usuario?> ObterUsuarioPorEmailAsync(string email) {

            return await _usuarioRepository.ObterUsuarioPorEmailAsync(email);
        }

        public async Task<Usuario?> ObterUsuarioPorIdAsync(int idUsuario) {

            return await _usuarioRepository.ObterUsuarioPorIdAsync(idUsuario);
        }

        public async Task<IEnumerable<Usuario>> ObterUsuariosAsync() {

            return await _usuarioRepository.ObterUsuariosAsync();
        }
    }
}
