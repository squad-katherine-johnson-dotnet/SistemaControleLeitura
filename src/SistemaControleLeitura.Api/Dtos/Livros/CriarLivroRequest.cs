using System.ComponentModel.DataAnnotations;

namespace SistemaControleLeitura.Api.Dtos.Livros;

public class CriarLivroRequest
{
    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Autor { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int NumeroPaginas { get; set; }
}