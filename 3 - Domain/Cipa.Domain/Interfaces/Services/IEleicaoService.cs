using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Domain.Interfaces.Services {
    public interface IEleicaoService: IServiceBase<Eleicao>
    {
        IEnumerable<Eleicao> BuscarPelaConta(Conta conta);
        IEnumerable<Eleicao> BuscarPeloUsuario(Usuario usuario);
        Dimensionamento CalcularDimensionamentoEleicao(Eleicao eleicao);

    }
}