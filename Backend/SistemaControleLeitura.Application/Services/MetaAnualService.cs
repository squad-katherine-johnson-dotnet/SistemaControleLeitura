using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Interfaces;
using SistemaControleLeitura.Domain.Entities;
using SistemaControleLeitura.Domain.Interfaces;

namespace SistemaControleLeitura.Application.Services;

public class MetaAnualService : IMetaAnualService
{
    private readonly IMetaAnualRepository _metaAnualRepository;

    public MetaAnualService(IMetaAnualRepository metaAnualRepository)
    {
        _metaAnualRepository = metaAnualRepository;
    }

    public async Task<MetaAnualRespostaDto> DefinirMetaAnualAsync(int usuarioId, int ano, DefinirMetaAnualDto metaDto)
    {
        if (ano < 2000 || ano > 2100)
        {
            throw new ArgumentException("Informe um ano válido.");
        }

        if (metaDto.QuantidadeMeta <= 0)
        {
            throw new ArgumentException("A quantidade de livros deve ser maior que zero.");
        }

        var metaExistente = await _metaAnualRepository.ObterPorUsuarioEAnoAsync(usuarioId, ano);

        if (metaExistente != null)
        {
            metaExistente.QuantidadeMeta = metaDto.QuantidadeMeta;
            await _metaAnualRepository.AtualizarAsync(metaExistente);

            return new MetaAnualRespostaDto
            {
                Id = metaExistente.Id,
                UsuarioId = metaExistente.UsuarioId,
                Ano = metaExistente.Ano,
                QuantidadeMeta = metaExistente.QuantidadeMeta
            };
        }

        var novaMeta = new MetaAnual
        {
            UsuarioId = usuarioId,
            Ano = ano,
            QuantidadeMeta = metaDto.QuantidadeMeta
        };

        await _metaAnualRepository.AdicionarAsync(novaMeta);

        return new MetaAnualRespostaDto
        {
            Id = novaMeta.Id,
            UsuarioId = novaMeta.UsuarioId,
            Ano = novaMeta.Ano,
            QuantidadeMeta = novaMeta.QuantidadeMeta
        };
    }

    public async Task<ProgressoMetaAnualDto> ConsultarProgressoAsync(int usuarioId, int ano)
    {
        if (ano < 2000 || ano > 2100)
        {
            throw new ArgumentException("Informe um ano válido.");
        }

        var meta = await _metaAnualRepository.ObterPorUsuarioEAnoAsync(usuarioId, ano);

        if (meta == null)
        {
            throw new KeyNotFoundException("Meta anual não encontrada para o ano informado.");
        }

        int livrosLidos = await _metaAnualRepository.ContarLivrosLidosNoAnoAsync(usuarioId, ano);

        int livrosRestantes = meta.QuantidadeMeta - livrosLidos;
        if (livrosRestantes < 0)
        {
            livrosRestantes = 0;
        }

        double percentualBruto = ((double)livrosLidos / meta.QuantidadeMeta) * 100.0;
        double percentualConcluido = Math.Round(percentualBruto, 1);

        double mediaMensalNecessaria = 0.0;

        if (livrosRestantes > 0)
        {
            int mesAtual = DateTime.Now.Month;
            int anoAtual = DateTime.Now.Year;
            int mesesRestantes = 0;

            if (ano < anoAtual) {
                mesesRestantes = 0;
            }
            else if (ano == anoAtual) {
                mesesRestantes = (12 - mesAtual) + 1;
            }
            else if (ano > anoAtual) {
                mesesRestantes = 12;
            }

            if (mesesRestantes > 0) {
                double mediaBruta = (double)livrosRestantes / mesesRestantes;
                mediaMensalNecessaria = Math.Round(mediaBruta, 1);
            }
        }

        return new ProgressoMetaAnualDto
        {
            Ano = ano,
            QuantidadeMeta = meta.QuantidadeMeta,
            LivrosLidos = livrosLidos,
            LivrosRestantes = livrosRestantes,
            PercentualConcluido = percentualConcluido,
            MediaMensalNecessaria = mediaMensalNecessaria
        };
    }
}