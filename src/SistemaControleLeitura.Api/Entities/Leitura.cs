namespace SistemaControleLeitura.Api.Entities;

public class Leitura
{
    public int Id { get; set; }
    public int LivroId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime? DataTermino { get; set; }
}