using Cipa.Domain.Entities;
using System.Collections.Generic;

namespace Cipa.Application.Interfaces
{
    public interface IEstabelecimentoAppService : IAppServiceBase<Estabelecimento>
    {
        IEnumerable<Estabelecimento> BuscarEstabelecimentosPorConta(int contaId);
        IEnumerable<Estabelecimento> BuscarEstabelecimentosPorEmpresa(int empresaId);
    }
}