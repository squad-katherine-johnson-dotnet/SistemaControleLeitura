using SistemaControleLeitura.Domain.Enums;

namespace SistemaControleLeitura.Domain.Entities;

public class Leitura
{
    public int Id { get; set; }
    public int ItemEstanteId { get; set; }
    public int ProgressoAtual { get; set; } = 0;
    public StatusLeitura Status { get; set; } = StatusLeitura.QueroLer;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataTermino { get; set; }
}
