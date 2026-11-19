using Microsoft.AspNetCore.Identity;
using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Interfaces;
using SistemaControleLeitura.Domain.Entities;

namespace SistemaControleLeitura.Application.Services {
    public class LoginService {

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;   

        public LoginService(IUsuarioRepository usuarioRepository, ITokenService tokenService) {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
        }

        public async Task<string> AutenticarAsync(LoginDto loginDto) {

            var usuario = await _usuarioRepository.ObterUsuarioPorEmailAsync(loginDto.Email);

            if (usuario == null) 
                throw new UnauthorizedAccessException("Email ou senha inválidos.");

            var resultado = new PasswordHasher<Usuario>()
                .VerifyHashedPassword(usuario, usuario.SenhaHash, loginDto.Senha);

           if (resultado == PasswordVerificationResult.Failed) 
                throw new UnauthorizedAccessException("Email ou senha inválidos.");

           return _tokenService.GerarToken(usuario);
        }
    }
}
