namespace SistemaControleLeitura.Application.DTOs
{
    public class LivroRespostaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;

        public int NumeroPaginas { get; set; }
    }
}