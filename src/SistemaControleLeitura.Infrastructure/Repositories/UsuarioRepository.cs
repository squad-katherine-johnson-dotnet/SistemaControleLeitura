using SistemaControleLeitura.Application.Interfaces;
using SistemaControleLeitura.Domain.Entities;
using SistemaControleLeitura.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SistemaControleLeitura.Infrastructure.Repositories {
    public class UsuarioRepository : IUsuarioRepository {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context) {
            _context = context;
        }

        public async Task CadastrarUsuarioAsync(Usuario usuario, CancellationToken cancellationToken = default) {

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Usuario?> ObterUsuarioPorIdAsync(int idUsuario, CancellationToken cancellationToken = default) {

            return await _context.Usuarios.FirstOrDefaultAsync(usuario => usuario.IdUsuario == idUsuario, cancellationToken);
        }

        public async Task<Usuario?> ObterUsuarioPorEmailAsync(string email, CancellationToken cancellationToken = default) {

            return await _context.Usuarios.FirstOrDefaultAsync(usuario => usuario.Email == email, cancellationToken);
        }

        public async Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default) {

            return await _context.Usuarios.AnyAsync(u => u.Email == email, cancellationToken);
        }
    }
}