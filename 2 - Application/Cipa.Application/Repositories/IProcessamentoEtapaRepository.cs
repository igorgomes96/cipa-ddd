using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Application.Repositories
{
    public interface IProcessamentoEtapaRepository : IRepositoryBase<ProcessamentoEtapa>
    {
        IEnumerable<ProcessamentoEtapa> BuscarProcessamentosPendentes();
    }
}