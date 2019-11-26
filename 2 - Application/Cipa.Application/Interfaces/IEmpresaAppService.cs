using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Application.Interfaces {
    public interface IEmpresaAppService: IAppServiceBase<Empresa>
    {
        IEnumerable<Empresa> BuscaEmpresasPorConta(int contaId, bool? ativa = true);
    }
}