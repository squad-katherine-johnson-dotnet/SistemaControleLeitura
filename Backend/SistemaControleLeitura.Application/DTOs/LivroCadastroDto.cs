using System.ComponentModel.DataAnnotations;

namespace SistemaControleLeitura.Application.DTOs
{
    public class LivroCadastroDto
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(120, MinimumLength = 2, ErrorMessage = "O título deve conter entre 2 e 120 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O autor é obrigatório.")]
        [StringLength(120, MinimumLength = 2, ErrorMessage = "O autor deve conter entre 2 e 120 caracteres.")]
        public string Autor { get; set; } = string.Empty;
      
        [Range(1, int.MaxValue, ErrorMessage = "O número de páginas deve ser maior que zero.")]
        public int NumeroPaginas { get; set; }
    }
}