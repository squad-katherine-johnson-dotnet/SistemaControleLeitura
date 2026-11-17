namespace SistemaControleLeitura.Domain.Entities;

public class MetaAnual
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int Ano { get; set; }
    public int QuantidadeMeta { get; set; }
}