namespace SistemaControleLeitura.Application.DTOs;

public class EstanteItemDto
{
    public int ItemEstanteId { get; set; }

    public int LivroId { get; set; }

    public int LeituraId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Autor { get; set; } = string.Empty;

    public int NumeroPaginas { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime DataAdicao { get; set; }

    public DateTime? DataInicio { get; set; }

    public DateTime? DataTermino { get; set; }
}