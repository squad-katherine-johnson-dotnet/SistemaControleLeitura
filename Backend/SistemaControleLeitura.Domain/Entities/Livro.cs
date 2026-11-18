namespace SistemaControleLeitura.Domain.Entities;

public class Livro
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public int NumeroPaginas { get; set; }
}