namespace SistemaControleLeitura.Application.DTOs;

public class ProgressoMetaAnualDto
{
    public int Ano { get; set; }
    public int QuantidadeMeta { get; set; }
    public int LivrosLidos { get; set; }
    public int LivrosRestantes { get; set; }
    public double PercentualConcluido { get; set; }
    public double MediaMensalNecessaria { get; set; }
}
