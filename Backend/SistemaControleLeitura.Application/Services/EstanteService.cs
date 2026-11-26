using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Interfaces;
using SistemaControleLeitura.Domain.Entities;
using SistemaControleLeitura.Domain.Enums;
using SistemaControleLeitura.Domain.Interfaces;

namespace SistemaControleLeitura.Application.Services;

public class EstanteService : IEstanteService
{
    private readonly IEstanteRepository _repository;

    public EstanteService(IEstanteRepository repository)
    {
        _repository = repository;
    }

    private static string FormatarStatus(StatusLeitura status) {
        return status switch {
            StatusLeitura.QueroLer => "Quero ler",
            StatusLeitura.Lendo => "Lendo",
            StatusLeitura.Lido => "Lido",
            _ => status.ToString()
        };
    }

    public async Task<EstanteItemDto> AdicionarLivroAsync(
        int livroId,
        int usuarioId) 
    { 

        if (livroId <= 0)
            throw new ArgumentException("ID do livro inválido.");

        var livro = await _repository.ObterLivroDoUsuarioAsync(livroId, usuarioId);

        if (livro == null)
            throw new KeyNotFoundException(
                "Livro não encontrado para este usuário.");

        var jaExiste = await _repository.LivroJaEstaNaEstanteAsync(
            livroId);

        if (jaExiste)
            throw new InvalidOperationException(
                "Este livro já está na estante.");

        var item = new ItemEstante
        {
            LivroId = livroId,
            DataAdicao = DateTime.Now
        };

        var leitura = new Leitura
        {
            Status = StatusLeitura.QueroLer
        };

        await _repository.AdicionarItemComLeituraAsync(
            item, leitura);

        return new EstanteItemDto {
            ItemEstanteId = item.Id,
            LivroId = livro.Id,
            LeituraId = leitura.Id,
            Titulo = livro.Titulo,
            Autor = livro.Autor,
            NumeroPaginas = livro.NumeroPaginas,
            Status = FormatarStatus(leitura.Status),
            DataAdicao = item.DataAdicao,
            DataInicio = leitura.DataInicio,
            DataTermino = leitura.DataTermino
        };
    }

    public async Task<EstanteItemDto> AlterarStatusAsync(
        int leituraId,
        int usuarioId,
        AtualizarStatusLeituraDto dto)
    {

        if (!Enum.TryParse<StatusLeitura>(
            dto.Status, true, out var novoStatus) ||
            !Enum.IsDefined(novoStatus))
        {
            throw new ArgumentException(
                "Status de leitura inválido.");
        }

        var leitura = await _repository.ObterLeituraDoUsuarioAsync(
            leituraId, usuarioId);

        if (leitura == null)
            throw new KeyNotFoundException(
                "Leitura não encontrada.");

        if (leitura.Status == StatusLeitura.Lido)
            throw new InvalidOperationException(
                "Uma leitura concluída não pode voltar de status. Inicie uma releitura.");

        if (leitura.Status == StatusLeitura.QueroLer &&
            novoStatus != StatusLeitura.Lendo)
        {
            throw new InvalidOperationException(
                "A leitura precisa passar pelo status Lendo.");
        }

        if (leitura.Status == StatusLeitura.Lendo &&
            novoStatus != StatusLeitura.Lido)
        {
            throw new InvalidOperationException(
                "A leitura precisa avançar para Lido.");
        }

        if (novoStatus == StatusLeitura.Lendo)
        {
            if (!dto.DataInicio.HasValue)
                throw new ArgumentException(
                    "A data de início é obrigatória.");

            leitura.DataInicio = dto.DataInicio.Value;
        }

        if (novoStatus == StatusLeitura.Lido)
        {
            if (!dto.DataTermino.HasValue)
                throw new ArgumentException(
                    "A data de término é obrigatória.");

            var dataInicio = leitura.DataInicio ?? dto.DataInicio;

            if (!dataInicio.HasValue)
                throw new ArgumentException(
                    "A data de início é obrigatória para concluir a leitura.");

            if (dto.DataTermino.Value.Date < dataInicio.Value.Date)
                throw new ArgumentException(
                    "A data de término não pode ser anterior à data de início.");

            leitura.DataInicio = dataInicio;
            leitura.DataTermino = dto.DataTermino.Value;
        }

        leitura.Status = novoStatus;

        await _repository.SalvarAlteracoesAsync();

        var resultado = await _repository.ObterItemComUltimaLeituraAsync(
        leitura.ItemEstanteId, usuarioId);

        if (resultado.Item == null || resultado.Livro == null)
            throw new KeyNotFoundException(
                "Livro não encontrado na estante.");

        return new EstanteItemDto {
            ItemEstanteId = resultado.Item.Id,
            LivroId = resultado.Livro.Id,
            LeituraId = leitura.Id,
            Titulo = resultado.Livro.Titulo,
            Autor = resultado.Livro.Autor,
            NumeroPaginas = resultado.Livro.NumeroPaginas,
            Status = FormatarStatus(leitura.Status),
            DataAdicao = resultado.Item.DataAdicao,
            DataInicio = leitura.DataInicio,
            DataTermino = leitura.DataTermino
        };
    }

    public async Task<EstanteItemDto> IniciarReleituraAsync(
        int itemId,
        int usuarioId) 
    { 

        var resultado = await _repository
            .ObterItemComUltimaLeituraAsync(itemId, usuarioId);

        if (resultado.Item == null ||
            resultado.Livro == null)
        {
            throw new KeyNotFoundException(
                "Livro não encontrado na estante.");
        }

        if (resultado.UltimaLeitura == null ||
            resultado.UltimaLeitura.Status != StatusLeitura.Lido)
        {
            throw new InvalidOperationException(
                "Só é possível iniciar uma releitura após concluir a leitura anterior.");
        }

        var novaLeitura = new Leitura
        {
            ItemEstanteId = itemId,
            Status = StatusLeitura.QueroLer,
            DataInicio = null,
            DataTermino = null
        };

        await _repository.AdicionarNovaLeituraAsync(novaLeitura);

        return new EstanteItemDto {
            ItemEstanteId = resultado.Item.Id,
            LivroId = resultado.Livro.Id,
            LeituraId = novaLeitura.Id,
            Titulo = resultado.Livro.Titulo,
            Autor = resultado.Livro.Autor,
            NumeroPaginas = resultado.Livro.NumeroPaginas,
            Status = FormatarStatus(novaLeitura.Status),
            DataAdicao = resultado.Item.DataAdicao,
            DataInicio = novaLeitura.DataInicio,
            DataTermino = novaLeitura.DataTermino
        };
    }

    public async Task<List<EstanteItemDto>> ListarEstanteAsync(
        int usuarioId,
        string? status,
        int? ano) {
        StatusLeitura? statusFiltro = null;

        if (!string.IsNullOrWhiteSpace(status)) {
            if (!Enum.TryParse<StatusLeitura>(status, true, out var statusConvertido) ||
                !Enum.IsDefined(statusConvertido)) {
                throw new ArgumentException("Status de leitura inválido.");
            }

            statusFiltro = statusConvertido;
        }

        if (ano.HasValue && (ano.Value < 1 || ano.Value > 9999))
            throw new ArgumentException("Ano inválido.");

        var itens = await _repository.ListarEstanteAsync(
            usuarioId, statusFiltro, ano);

        return itens.Select(registro => new EstanteItemDto {
            ItemEstanteId = registro.Item.Id,
            LivroId = registro.Livro.Id,
            LeituraId = registro.UltimaLeitura.Id,
            Titulo = registro.Livro.Titulo,
            Autor = registro.Livro.Autor,
            NumeroPaginas = registro.Livro.NumeroPaginas,
            Status = FormatarStatus(registro.UltimaLeitura.Status),
            DataAdicao = registro.Item.DataAdicao,
            DataInicio = registro.UltimaLeitura.DataInicio,
            DataTermino = registro.UltimaLeitura.DataTermino
        }).ToList();
    }

    public async Task RemoverLivroAsync(int itemId, int usuarioId)
    {
        var resultado = await _repository
            .ObterItemComUltimaLeituraAsync(itemId, usuarioId);

        if (resultado.Item == null)
            throw new KeyNotFoundException(
                "Livro não encontrado na estante.");

        await _repository.RemoverItemAsync(resultado.Item);
    }
}