using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Domain.Interfaces.Repositories {
    public interface IEmpresaRepository: IRepositoryBase<Empresa>
    {
        IEnumerable<Empresa> BuscarEmpresasPorConta(int contaId, bool? ativa = true);
    }
}