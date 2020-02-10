using Cipa.Domain.Entities;
using System.Collections.Generic;

namespace Cipa.Application.Interfaces
{
    public interface IEmpresaAppService : IAppServiceBase<Empresa>
    {
        IEnumerable<Empresa> BuscaEmpresasPorConta(int contaId, bool? ativa = true);
    }
}