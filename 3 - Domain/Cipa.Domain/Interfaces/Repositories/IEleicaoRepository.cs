using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Domain.Interfaces.Repositories {
    public interface IEleicaoRepository: IRepositoryBase<Eleicao>
    {
        IEnumerable<Eleicao> BuscarPelaConta(Conta conta);
        IEnumerable<Eleicao> BuscarPeloUsuario(Usuario usuario);
        IEnumerable<Eleitor> BuscarEleitores(Eleicao eleicao);
        IEnumerable<Voto> BuscarRegistrosVotos(Eleicao eleicao);
        IEnumerable<Inscricao> BuscarInscricoes(Eleicao eleicao);
        int QtdaEleitores(Eleicao eleicao);
        int QtdaVotos(Eleicao eleicao);
    }
}