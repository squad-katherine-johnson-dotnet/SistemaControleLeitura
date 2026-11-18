namespace SistemaControleLeitura.Application.DTOs {
    public class RespostaDto<T> {

        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public T? Dados { get; set; }
    }
}

