namespace SistemaControleLeitura.Application.DTOs
{
    // Os três desfechos possíveis de uma exclusão.
    // O Controller traduz cada um em um código HTTP: 200, 404 ou 409.
    public enum ResultadoExclusaoLivro
    {
        Excluido,
        NaoEncontrado,
        NaEstante
    }
}
