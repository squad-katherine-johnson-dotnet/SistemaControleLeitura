using Microsoft.AspNetCore.Mvc;

namespace SistemaControleLeitura.Api.Controllers;

public abstract class BaseController : ControllerBase {
    protected int UsuarioId { get; } = 1;
    // PROVISÓRIO: trocar pelo token
}
