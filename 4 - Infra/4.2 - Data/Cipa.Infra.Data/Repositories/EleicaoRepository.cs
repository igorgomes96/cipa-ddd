using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Domain.Entities;
using Cipa.Infra.Data.Context;

namespace Cipa.Infra.Data.Repositories
{
    public class EleicaoRepository : RepositoryBase<Eleicao>, IEleicaoRepository
    {
        public EleicaoRepository(CipaContext db) : base(db) { }

        private IQueryable<Eleicao> QueryEleicao()
        {
            return DbSet
                .Include(e => e.Dimensionamento)
                .Include(e => e.Grupo)
                    .ThenInclude(g => g.Dimensionamentos)
                .Include(e => e.Grupo)
                    .ThenInclude(g => g.LimiteDimensionamento)
                .Include(e => e.Usuario)
                .Include(e => e.Estabelecimento)
                    .ThenInclude(e => e.Empresa)
                .Include(e => e.Cronograma)
                    .ThenInclude(e => e.EtapaObrigatoria)
                .Include(e => e.Inscricoes)
                    .ThenInclude(e => e.Eleitor)
                .Include(e => e.Inscricoes)
                    .ThenInclude(e => e.Reprovacoes);
        }

        public override IEnumerable<Eleicao> BuscarTodos()
        {
            var eleicoes = QueryEleicao();
            eleicoes.Load();
            return eleicoes;
        }

        public override Eleicao BuscarPeloId(int id)
        {
            return QueryEleicao().SingleOrDefault(e => e.Id == id);
        }

        public Eleicao BuscarPeloIdCarregarEleitores(int id)
        {
            return QueryEleicao()
                .Include(e => e.Eleitores)
                    .ThenInclude(e => e.Usuario)
                .SingleOrDefault(e => e.Id == id);
        }

        public Eleicao BuscarPeloIdCarregarVotos(int id)
        {
            return QueryEleicao()
                .Include(e => e.Votos)
                    .ThenInclude(v => v.Eleitor)
                .SingleOrDefault(e => e.Id == id);
        }

        public IEnumerable<Eleicao> BuscarPelaConta(int contaId)
        {
            var eleicoes = QueryEleicao().Where(e => e.ContaId == contaId);
            eleicoes.Load();
            return eleicoes;
        }

        public IEnumerable<Eleicao> BuscarPeloUsuario(int usuarioId)
        {
            var eleicoes = QueryEleicao().Where(e => e.UsuarioCriacaoId == usuarioId);
            eleicoes.Load();
            return eleicoes;
        }

        public Eleitor BuscarEleitor(Eleicao eleicao, int id)
        {
            return _db.Eleitores
                .Include(e => e.Usuario)
                .Include(e => e.Inscricao)
                    .ThenInclude(i => i.Reprovacoes)
                .Include(e => e.Voto)
                .SingleOrDefault(e => e.EleicaoId == eleicao.Id && e.Id == id);
        }

        public IEnumerable<Inscricao> BuscarInscricoes(int eleicaoId, StatusInscricao? status = null)
        {
            var eleicoes = _db.Inscricoes
                .Include(i => i.Eleitor)
                .Include(i => i.Reprovacoes).Where(i => i.EleicaoId == eleicaoId);
            if (!status.HasValue) return eleicoes;
            return eleicoes.Where(i => i.StatusInscricao == status);
        }

        public IEnumerable<Eleitor> BuscarEleitores(int id)
        {
            return _db.Eleitores.Where(e => e.EleicaoId == id);
        }

        public Eleicao BuscarPeloIdCarregarTodoAgregado(int id)
        {
            return QueryEleicao()
                .Include(e => e.Eleitores)
                    .ThenInclude(e => e.Usuario)
                .Include(e => e.Votos).SingleOrDefault(e => e.Id == id);
        }

        public IEnumerable<Voto> BuscarVotos(int id)
        {
            return _db.Votos.Where(v => v.EleicaoId == id);
        }
    }
}