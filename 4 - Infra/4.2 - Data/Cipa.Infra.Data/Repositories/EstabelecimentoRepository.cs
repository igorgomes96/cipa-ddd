using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Domain.Entities;
using Cipa.Infra.Data.Context;

namespace Cipa.Infra.Data.Repositories
{
    public class EstabelecimentoRepository : RepositoryBase<Estabelecimento>, IEstabelecimentoRepository
    {
        public EstabelecimentoRepository(CipaContext db) : base(db) { }

        public override Estabelecimento BuscarPeloId(int id) {
            return DbSet.Include(e => e.Empresa).Include(e => e.Grupo).SingleOrDefault(e => e.Id == id);
        }

        public int QuantidadeEleicoesAno(int estabelecimentoId, int ano)
        {
            return _db.Eleicoes.Count(e => e.EstabelecimentoId == estabelecimentoId && e.Gestao == ano);
        }
    }
}