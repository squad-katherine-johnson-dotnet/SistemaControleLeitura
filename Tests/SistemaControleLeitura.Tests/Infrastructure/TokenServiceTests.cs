using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SistemaControleLeitura.Domain.Entities;
using SistemaControleLeitura.Infrastructure.Services;

namespace SistemaControleLeitura.Tests.Infrastructure {
    public class TokenServiceTests {
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        private const string SecretKey = "MinhaChaveSecretaDeTesteComMaisDe32Bytes123!";

        public TokenServiceTests() {
            var settings = new Dictionary<string, string?> {
                ["Jwt:SecretKey"] = SecretKey,
                ["Jwt:Issuer"] = "ControleDeLeitura",
                ["Jwt:Audience"] = "ControleDeLeituraUsuarios",
                ["Jwt:ExpirationInMinutes"] = "60"
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            _tokenService = new TokenService(_configuration);
        }

        [Fact]
        public void GerarToken_QuandoConfiguracaoValida_DeveRetornarTokenJwt() {

            var usuario = new Usuario(
                "Maria",
                "maria@gmail.com",
                "hashSenha123"
            );

            var token = _tokenService.GerarToken(usuario);

            Assert.False(string.IsNullOrWhiteSpace(token));

            var handler = new JwtSecurityTokenHandler();

            Assert.True(handler.CanReadToken(token));
        }

        [Fact]
        public void GerarToken_DeveIncluirClaimsDoUsuario() {

            var usuario = new Usuario(
                "Maria",
                "maria@gmail.com",
                "hashSenha123"
            );

            var tokenString = _tokenService.GerarToken(usuario);
            var token = new JwtSecurityTokenHandler()
                .ReadJwtToken(tokenString);

            Assert.Equal(
                usuario.IdUsuario.ToString(),
                token.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
            );

            Assert.Equal(
                usuario.Email,
                token.Claims.First(c => c.Type == ClaimTypes.Email).Value
            );
        }

        [Fact]
        public void GerarToken_DeveConterIssuerEAudienceConfigurados() {

            var usuario = new Usuario(
                "Maria",
                "maria@gmail.com",
                "hashSenha123"
            );

            var tokenString = _tokenService.GerarToken(usuario);
            var token = new JwtSecurityTokenHandler()
                .ReadJwtToken(tokenString);

            Assert.Equal("ControleDeLeitura", token.Issuer);
            Assert.Contains("ControleDeLeituraUsuarios", token.Audiences);
        }

        [Fact]
        public void GerarToken_DeveDefinirExpiracaoConfigurada() {

            var usuario = new Usuario(
                "Maria",
                "maria@gmail.com",
                "hashSenha123"
            );

            var antesDaGeracao = DateTime.UtcNow;

            var tokenString = _tokenService.GerarToken(usuario);
            var token = new JwtSecurityTokenHandler()
                .ReadJwtToken(tokenString);

            var expiracaoEsperada = antesDaGeracao.AddMinutes(60);

            Assert.InRange(
                token.ValidTo,
                expiracaoEsperada.AddSeconds(-2),
                expiracaoEsperada.AddSeconds(2)
            );
        }

        [Fact]
        public void GerarToken_QuandoChaveSecretaNaoConfigurada_DeveLancarExcecao() {

            var configuracao = CriarConfiguracao(
                secretKey: null
            );

            var service = new TokenService(configuracao);

            var usuario = CriarUsuario();

            var exception = Assert.Throws<InvalidOperationException>(
                () => service.GerarToken(usuario)
            );

            Assert.Equal(
                "A chave secreta do JWT não foi configurada.",
                exception.Message
            );
        }

        [Fact]
        public void GerarToken_QuandoChaveSecretaMenorQue32Bytes_DeveLancarExcecao() {

            var configuracao = CriarConfiguracao(
                secretKey: "chave-curta"
            );

            var service = new TokenService(configuracao);

            var usuario = CriarUsuario();

            var exception = Assert.Throws<InvalidOperationException>(
                () => service.GerarToken(usuario)
            );

            Assert.Equal(
                "A chave secreta do JWT deve possuir pelo menos 32 bytes.",
                exception.Message
            );
        }

        [Theory]
        [InlineData(null, "ControleDeLeitura", "60",
            "O emissor do JWT não foi configurado.")]
        [InlineData("ControleDeLeitura", null, "60",
            "O público do JWT não foi configurado.")]
        [InlineData("ControleDeLeitura", "Usuarios", null,
            "O tempo de expiração do JWT é inválido.")]
        [InlineData("ControleDeLeitura", "Usuarios", "abc",
            "O tempo de expiração do JWT é inválido.")]
        [InlineData("ControleDeLeitura", "Usuarios", "0",
            "O tempo de expiração do JWT é inválido.")]
        public void GerarToken_QuandoConfiguracaoInvalida_DeveLancarExcecao(
            string? issuer,
            string? audience,
            string? expiration,
            string mensagemEsperada) {

            var configuracao = CriarConfiguracao(
                issuer: issuer,
                audience: audience,
                expiration: expiration
            );

            var service = new TokenService(configuracao);

            var usuario = CriarUsuario();

            var exception = Assert.Throws<InvalidOperationException>(
                () => service.GerarToken(usuario)
            );

            Assert.Equal(mensagemEsperada, exception.Message);
        }

        private static IConfiguration CriarConfiguracao(
            string? secretKey = SecretKey,
            string? issuer = "ControleDeLeitura",
            string? audience = "ControleDeLeituraUsuarios",
            string? expiration = "60") {
            var settings = new Dictionary<string, string?> {
                ["Jwt:SecretKey"] = secretKey,
                ["Jwt:Issuer"] = issuer,
                ["Jwt:Audience"] = audience,
                ["Jwt:ExpirationInMinutes"] = expiration
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        private static Usuario CriarUsuario() {
            return new Usuario(
                "Maria",
                "maria@gmail.com",
                "hashSenha123"
            );
        }
    }
}
