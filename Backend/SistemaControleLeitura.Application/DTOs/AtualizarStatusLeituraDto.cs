using System.ComponentModel.DataAnnotations;

namespace SistemaControleLeitura.Application.DTOs;

public class AtualizarStatusLeituraDto
{
    [Required(ErrorMessage = "O status é obrigatório.")]
    public string Status { get; set; } = string.Empty;

    public DateTime? DataInicio { get; set; }

    public DateTime? DataTermino { get; set; }
}