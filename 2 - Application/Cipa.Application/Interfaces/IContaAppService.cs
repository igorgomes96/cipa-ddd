using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Application.Interfaces {
    public interface IContaAppService: IAppServiceBase<Conta>
    {
        IEnumerable<EtapaPadraoConta> BuscarCronogramaPadrao(int contaId);
        EtapaPadraoConta AdicionarEtapaPadrao(int contaId, EtapaPadraoConta etapaPadrao);
        EtapaPadraoConta AtualizarEtapaPadrao(int contaId, EtapaPadraoConta etapaPadrao);
        EtapaPadraoConta RemoverEtapaPadrao(int contaId, int etapaPadraoId);
    }
}