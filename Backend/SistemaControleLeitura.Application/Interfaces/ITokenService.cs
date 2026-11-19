using SistemaControleLeitura.Domain.Entities;

namespace SistemaControleLeitura.Application.Interfaces {
    public interface ITokenService {

        string GerarToken(Usuario usuario);

    }
}
