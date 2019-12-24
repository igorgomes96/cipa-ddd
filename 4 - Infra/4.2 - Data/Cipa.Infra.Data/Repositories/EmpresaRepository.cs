using Cipa.Domain.Entities;
using Cipa.Application.Repositories;
using Cipa.Infra.Data.Context;
using System.Collections.Generic;
using System.Linq;

namespace Cipa.Infra.Data.Repositories
{
    public class EmpresaRepository : RepositoryBase<Empresa>, IEmpresaRepository
    {
        public EmpresaRepository(CipaContext db) : base(db) { }

        public IEnumerable<Empresa> BuscarEmpresasPorConta(int contaId, bool? ativa = true)
        {
            var pesquisa = DbSet.Where(e => e.ContaId == contaId);
            if (ativa.HasValue)
                return pesquisa.Where(e => e.Ativa == ativa.Value);

            return pesquisa;
        }


    }
}