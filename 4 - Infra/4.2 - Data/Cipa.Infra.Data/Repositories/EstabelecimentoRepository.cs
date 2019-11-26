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

        private IQueryable<Estabelecimento> QueryEstabelecimentos => 
            DbSet.Where(e => e.Ativo);

        public IEnumerable<Estabelecimento> BuscarEstabelecimentosPorConta(int contaId) =>
            QueryEstabelecimentos.Where(e => e.Empresa.ContaId == contaId);

        public IEnumerable<Estabelecimento> BuscarEstabelecimentosPorEmpresa(int empresaId) =>
            QueryEstabelecimentos.Where(e => e.EmpresaId == empresaId);

        public override Estabelecimento BuscarPeloId(int id) =>
            QueryEstabelecimentos.SingleOrDefault(e => e.Id == id);

    }
}