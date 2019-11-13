using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Application.Interfaces {
    public interface IEleicaoAppService: IAppServiceBase<Eleicao>
    {
        IEnumerable<Eleicao> BuscarPelaConta(int contaId);
        IEnumerable<Eleicao> BuscarPeloUsuario(int usuarioId);
    }
}