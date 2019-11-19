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
            return QueryEleicao().Include(e => e.Eleitores).SingleOrDefault(e => e.Id == id);
        }

        public Eleicao BuscarPeloIdCarregarVotos(int id)
        {
            return QueryEleicao().Include(e => e.Votos).SingleOrDefault(e => e.Id == id);
        }

        public IEnumerable<Eleicao> BuscarPelaConta(Conta conta)
        {
            var eleicoes = QueryEleicao().Where(e => e.ContaId == conta.Id);
            eleicoes.Load();
            return eleicoes;
        }

        public IEnumerable<Eleicao> BuscarPeloUsuario(Usuario usuario)
        {
            var eleicoes = QueryEleicao().Where(e => e.UsuarioCriacaoId == usuario.Id);
            eleicoes.Load();
            return eleicoes;
        }

        public int QtdaEleitores(Eleicao eleicao)
        {
            return _db.Inscricoes.Count(i => i.EleicaoId == eleicao.Id);
        }

        public int QtdaVotos(Eleicao eleicao)
        {
            return _db.Votos.Count(v => v.EleicaoId == eleicao.Id);
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

    }
}