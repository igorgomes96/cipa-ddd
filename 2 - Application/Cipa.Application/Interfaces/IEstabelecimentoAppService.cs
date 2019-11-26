using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Application.Interfaces {
    public interface IEstabelecimentoAppService: IAppServiceBase<Estabelecimento>
    {
        IEnumerable<Estabelecimento> BuscarEstabelecimentosPorConta(int contaId);
        IEnumerable<Estabelecimento> BuscarEstabelecimentosPorEmpresa(int empresaId);
    }
}