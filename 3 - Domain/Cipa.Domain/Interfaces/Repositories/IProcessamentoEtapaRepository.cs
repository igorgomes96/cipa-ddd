using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Domain.Interfaces.Repositories
{
    public interface IProcessamentoEtapaRepository : IRepositoryBase<ProcessamentoEtapa>
    {
        IEnumerable<ProcessamentoEtapa> BuscarProcessamentosPendentes();
    }
}