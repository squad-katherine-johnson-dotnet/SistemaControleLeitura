using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SistemaControleLeitura.Application.Interfaces;
using SistemaControleLeitura.Domain.Entities;

namespace SistemaControleLeitura.Infrastructure.Services {
    public class TokenService : ITokenService {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration) {
            _configuration = configuration;
        }

        public string GerarToken(Usuario usuario) {

            var secretKey = _configuration["Jwt:SecretKey"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expiration = _configuration["Jwt:ExpirationInMinutes"];

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("A chave secreta do JWT não foi configurada.");

            if (Encoding.UTF8.GetByteCount(secretKey) < 32)
                throw new InvalidOperationException("A chave secreta do JWT deve possuir pelo menos 32 bytes.");

            if (string.IsNullOrWhiteSpace(issuer))
                throw new InvalidOperationException("O emissor do JWT não foi configurado.");

            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("O público do JWT não foi configurado.");

            if (!int.TryParse(expiration, out var expirationMinutes) || expirationMinutes <= 0)
                throw new InvalidOperationException("O tempo de expiração do JWT é inválido.");

            var claims = new List<Claim> {
                new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new(ClaimTypes.Email, usuario.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var expirationDate = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expirationDate,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
