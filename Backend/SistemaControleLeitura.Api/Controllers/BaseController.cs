using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SistemaControleLeitura.Api.Controllers;

public abstract class BaseController : ControllerBase {
    protected int UsuarioId {

        get {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            return usuarioId;
        }
    }
}
