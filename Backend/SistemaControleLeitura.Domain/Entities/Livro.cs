namespace SistemaControleLeitura.Domain.Entities;

public class Livro
{
    public int Id { get; private set; }
    public int UsuarioId { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Autor { get; private set; } = string.Empty;
    public int NumeroPaginas { get; private set; }

    public Livro(int usuarioId, string titulo, string autor, int numeroPaginas)
    {
        ValidarUsuario(usuarioId);
        ValidarTitulo(titulo);
        ValidarAutor(autor);
        ValidarNumeroPaginas(numeroPaginas);

        UsuarioId = usuarioId;
        Titulo = titulo.Trim();
        Autor = autor.Trim();
        NumeroPaginas = numeroPaginas;

    }

    private static void ValidarUsuario(int usuarioId)
    {
        if (usuarioId <= 0)
        {
            throw new ArgumentException("O Id de Usuário deve ser maior que zero.");
        }
    }

    private static void ValidarTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("O título deve conter entre 2 e 120 caracteres.");

        var tamanho = titulo.Trim().Length;

        if (tamanho < 2 || tamanho > 120)
            throw new ArgumentException("O título deve conter entre 2 e 120 caracteres.");
    }

    private static void ValidarAutor(string autor)
    {
        if (string.IsNullOrWhiteSpace(autor))
            throw new ArgumentException("O autor deve conter entre 2 e 120 caracteres.");

        var tamanho = autor.Trim().Length;

        if (tamanho < 2 || tamanho > 120)
            throw new ArgumentException("O autor deve conter entre 2 e 120 caracteres.");
    }

    private static void ValidarNumeroPaginas(int numeroPaginas)
    {
        if (numeroPaginas <= 0)
        {
            throw new ArgumentException("O número de páginas deve ser maior que zero.");
        }
    }
}