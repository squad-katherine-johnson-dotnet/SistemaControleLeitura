using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Interfaces;
using SistemaControleLeitura.Domain.Entities;
using SistemaControleLeitura.Domain.Enums;

namespace SistemaControleLeitura.Application.Services;

public class EstanteService : IEstanteService
{
    private readonly IEstanteRepository _repository;

    public EstanteService(IEstanteRepository repository)
    {
        _repository = repository;
    }

    public async Task AdicionarLivroAsync(
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
    }

    public async Task AlterarStatusAsync(int leituraId, int usuarioId, AtualizarStatusLeituraDto dto)
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
    }

    public async Task IniciarReleituraAsync(int itemId, int usuarioId)
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
    }

    public async Task<List<EstanteItemDto>> ListarEstanteAsync(int usuarioId, string? status, int? ano)
    {
        if (!string.IsNullOrWhiteSpace(status) &&
            !Enum.TryParse<StatusLeitura>(status, true, out _))
        {
            throw new ArgumentException(
                "Status de leitura inválido.");
        }

        if (ano.HasValue && (ano.Value < 1 || ano.Value > 9999))
            throw new ArgumentException("Ano inválido.");

        return await _repository.ListarEstanteAsync(
            usuarioId, status, ano);
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