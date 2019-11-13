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
                .Include(e => e.Usuario)
                .Include(e => e.Estabelecimento)
                    .ThenInclude(e => e.Empresa)
                .Include(e => e.Cronograma)
                    .ThenInclude(e => e.EtapaObrigatoria);
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

        public IEnumerable<Eleitor> BuscarEleitores(Eleicao eleicao)
        {
            return _db.Eleitores.Where(e => e.EleicaoId == eleicao.Id);
        }

        public IEnumerable<Voto> BuscarRegistrosVotos(Eleicao eleicao)
        {
            return _db.Votos.Where(e => e.EleicaoId == eleicao.Id);
        }

        public int QtdaEleitores(Eleicao eleicao)
        {
            return _db.Inscricoes.Count(i => i.EleicaoId == eleicao.Id);
        }

        public IEnumerable<Inscricao> BuscarInscricoes(Eleicao eleicao)
        {
            return _db.Inscricoes.Where(i => i.EleicaoId == eleicao.Id);
        }

        public int QtdaVotos(Eleicao eleicao)
        {
            return _db.Votos.Count(v => v.EleicaoId == eleicao.Id);
        }
    }
}