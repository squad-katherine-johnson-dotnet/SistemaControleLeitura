namespace SistemaControleLeitura.Application.DTOs;

public class AtualizarStatusLeituraDto
{
    public string Status { get; set; } = string.Empty;

    public DateTime? DataInicio { get; set; }

    public DateTime? DataTermino { get; set; }
}