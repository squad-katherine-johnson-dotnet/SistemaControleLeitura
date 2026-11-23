using SistemaControleLeitura.Application.DTOs;
using SistemaControleLeitura.Application.Interfaces;
using SistemaControleLeitura.Domain.Entities;
using SistemaControleLeitura.Domain.Interfaces;

namespace SistemaControleLeitura.Application.Services
{
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
    }
}