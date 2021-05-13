using Cipa.Domain.Entities;
using Cipa.Application.Repositories;
using Cipa.Infra.Data.Context;
using System.Collections.Generic;
using System.Linq;
using System;
using Cipa.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Cipa.Domain.Helpers;

namespace Cipa.Infra.Data.Repositories
{
    public class EleicaoRepository : RepositoryBase<Eleicao>, IEleicaoRepository
    {
        public EleicaoRepository(CipaContext db) : base(db) { }

        public IEnumerable<Eleicao> BuscarPelaConta(int contaId) => DbSet.Where(e => e.ContaId == contaId);

        public IEnumerable<Eleicao> BuscarPeloUsuario(int usuarioId) =>
            DbSet.Where(e => e.Eleitores.Any(eleitor => eleitor.UsuarioId == usuarioId));

        public IEnumerable<Inscricao> BuscarInscricoes(int eleicaoId, StatusInscricao? status = null)
        {
            var eleicoes = _db.Inscricoes.Where(i => i.EleicaoId == eleicaoId);
            if (!status.HasValue) return eleicoes;
            return eleicoes.Where(i => i.StatusInscricao == status);
        }

        public IEnumerable<Eleitor> BuscarEleitores(int id) => _db.Eleitores.Where(e => e.EleicaoId == id);

        public IEnumerable<Voto> BuscarVotos(int id) => _db.Votos.Where(v => v.EleicaoId == id);

        public bool VerificarSeUsuarioEhEleitor(int eleicaoId, int usuarioId) =>
            _db.Eleitores.Any(e => e.EleicaoId == eleicaoId && e.UsuarioId == usuarioId);

        public IEnumerable<Eleicao> BuscarEleicoesComMudancaEtapaAgendadaParaHoje()
        {
            var dataInicio = DateTime.Now.HorarioBrasilia().Date.AddDays(-2);  // Intervalo máximo de 2 dias
            var eleicoesAndamento = _db.Eleicoes
                .Include(e => e.Cronograma)
                .Where(e => !e.DataFinalizacao.HasValue)
                .ToList();
            return eleicoesAndamento.Where(e =>
                (e.EtapaAtual == null 
                && e.Cronograma.All(c => c.PosicaoEtapa == EPosicaoEtapa.Futura)
                && e.Cronograma.First().DataPrevista.Date <= DateTime.Now.HorarioBrasilia().Date
                && e.Cronograma.First().DataPrevista.Date >= dataInicio)  // Início do Processo
                ||
                (e.EtapaAtual != null
                && string.IsNullOrWhiteSpace(e.EtapaAtual.ErroMudancaEtapa)
                && e.DataTerminoEtapa(e.EtapaAtual) <= DateTime.Now.HorarioBrasilia().Date
                && e.DataTerminoEtapa(e.EtapaAtual) >= dataInicio)); // Demais etapas após o início 
        }

    }
}