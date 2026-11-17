namespace SistemaControleLeitura.Domain.Entities;

public class ItemEstante
{
    public int Id { get; set; }
    public int LivroId { get; set; }
    public DateTime DataAdicao { get; set; }
}