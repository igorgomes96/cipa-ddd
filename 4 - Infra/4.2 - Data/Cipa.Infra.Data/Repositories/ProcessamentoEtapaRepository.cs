using System.Collections.Generic;
using System.Linq;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;
using Cipa.Application.Repositories;
using Cipa.Infra.Data.Context;

namespace Cipa.Infra.Data.Repositories
{
    public class ProcessamentoEtapaRepository : RepositoryBase<ProcessamentoEtapa>, IProcessamentoEtapaRepository
    {
        public ProcessamentoEtapaRepository(CipaContext db) : base(db) { }

        public IEnumerable<ProcessamentoEtapa> BuscarProcessamentosPendentes()
        {
            return DbSet.Where(p => p.StatusProcessamentoEtapa == EStatusProcessamentoEtapa.Pendente);
        }
    }
}