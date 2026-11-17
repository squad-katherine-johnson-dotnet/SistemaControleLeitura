namespace SistemaControleLeitura.Domain.Entities;

public class ItemEstante
{
    public int Id { get; set; }
    public int LivroId { get; set; }
    public string StatusAtual { get; set; } = string.Empty;
    public DateTime DataAdicao { get; set; }
}