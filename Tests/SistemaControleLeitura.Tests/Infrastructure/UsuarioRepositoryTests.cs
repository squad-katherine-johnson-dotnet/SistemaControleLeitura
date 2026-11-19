using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SistemaControleLeitura.Domain.Entities;
using SistemaControleLeitura.Infrastructure.Data;
using SistemaControleLeitura.Infrastructure.Repositories;

namespace SistemaControleLeitura.Tests.Infrastructure {
    public class UsuarioRepositoryTests : IDisposable {

        private readonly SqliteConnection _connection;
        private readonly AppDbContext _context;
        private readonly UsuarioRepository _repository;

        public UsuarioRepositoryTests() {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();

            _repository = new UsuarioRepository(_context);
        }

        public void Dispose() {
            _context.Dispose();
            _connection.Dispose();
        }

        private async Task<Usuario> SalvarUsuarioNoBancoAsync(
            string nome = "Maria Silva",
            string email = "maria@email.com",
            string senhaHash = "hash123") {
            var usuario = new Usuario(nome, email, senhaHash);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        [Fact]
        public async Task CadastrarUsuarioAsync_QuandoDadosValidos_DeveSalvarUsuario() {

            var usuario = new Usuario(
                "Maria Silva",
                "maria@email.com",
                "hash123");

            await _repository.CadastrarUsuarioAsync(usuario);

            var usuarioSalvo = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == "maria@email.com");

            Assert.NotNull(usuarioSalvo);
            Assert.True(usuarioSalvo.IdUsuario > 0);
            Assert.Equal(usuario.Nome, usuarioSalvo.Nome);
            Assert.Equal(usuario.Email, usuarioSalvo.Email);
        }

        [Fact]
        public async Task ObterUsuarioPorIdAsync_QuandoUsuarioExiste_DeveRetornarUsuario() {
            
            var usuario = await SalvarUsuarioNoBancoAsync();

            
            var resultado = await _repository.ObterUsuarioPorIdAsync(
                usuario.IdUsuario);

            
            Assert.NotNull(resultado);
            Assert.Equal(usuario.IdUsuario, resultado.IdUsuario);
            Assert.Equal(usuario.Email, resultado.Email);
        }

        [Fact]
        public async Task ObterUsuarioPorIdAsync_QuandoUsuarioNaoExiste_DeveRetornarNulo() {

            var idInexistente = 999;

            var resultado = await _repository.ObterUsuarioPorIdAsync(idInexistente);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObterUsuarioPorEmailAsync_QuandoEmailExiste_DeveRetornarUsuario() {

            var usuario = await SalvarUsuarioNoBancoAsync();

            var resultado = await _repository.ObterUsuarioPorEmailAsync(
                "  MARIA@EMAIL.COM  ");

            Assert.NotNull(resultado);
            Assert.Equal(usuario.IdUsuario, resultado.IdUsuario);
            Assert.Equal(usuario.Email, resultado.Email);
        }

        [Fact]
        public async Task ObterUsuarioPorEmailAsync_QuandoEmailNaoExiste_DeveRetornarNulo() {

            var emailInexistente = "inexistente@email.com";

            var resultado = await _repository.ObterUsuarioPorEmailAsync(
                emailInexistente);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ExisteEmailAsync_QuandoEmailExiste_DeveRetornarVerdadeiro() {

            await SalvarUsuarioNoBancoAsync();

            var resultado = await _repository.ExisteEmailAsync(
                "  MARIA@EMAIL.COM  ");

            Assert.True(resultado);
        }

        [Fact]
        public async Task ExisteEmailAsync_QuandoEmailNaoExiste_DeveRetornarFalso() {

            var emailInexistente = "inexistente@email.com";

            var resultado = await _repository.ExisteEmailAsync(
                emailInexistente);

            Assert.False(resultado);
        }

        [Fact]
        public async Task ObterUsuariosAsync_QuandoExistemUsuarios_DeveRetornarLista() {

            await SalvarUsuarioNoBancoAsync(
                "Maria Silva",
                "maria@email.com");

            await SalvarUsuarioNoBancoAsync(
                "Ana Souza",
                "ana@email.com");

            var resultado = await _repository.ObterUsuariosAsync();

            var usuarios = resultado.ToList();

            Assert.Equal(2, usuarios.Count);
            Assert.Contains(usuarios, u => u.Email == "maria@email.com");
            Assert.Contains(usuarios, u => u.Email == "ana@email.com");
        }

        [Fact]
        public async Task ObterUsuariosAsync_QuandoNaoExistemUsuarios_DeveRetornarListaVazia() {

            var resultado = await _repository.ObterUsuariosAsync();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

    }
}
