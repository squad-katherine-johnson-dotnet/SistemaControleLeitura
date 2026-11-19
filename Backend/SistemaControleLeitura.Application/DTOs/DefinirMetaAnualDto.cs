using System.ComponentModel.DataAnnotations;

namespace SistemaControleLeitura.Application.DTOs
{
    public class DefinirMetaAnualDto
    {
        [Required(ErrorMessage = "A quantidade de livros é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade de livros deve ser maior que zero.")]
        public int QuantidadeMeta { get; set; }
    }
}