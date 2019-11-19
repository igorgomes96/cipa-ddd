using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Domain.Interfaces.Repositories {
    public interface IEleicaoRepository: IRepositoryBase<Eleicao>
    {
        Eleicao BuscarPeloIdCarregarEleitores(int id);
        Eleicao BuscarPeloIdCarregarVotos(int id);
        Eleicao BuscarPeloIdCarregarTodoAgregado(int id);
        IEnumerable<Inscricao> BuscarInscricoes(int id, StatusInscricao? status = null);
        // IEnumerable<Voto> BuscarVotos(int id);
        IEnumerable<Eleitor> BuscarEleitores(int id);
        IEnumerable<Eleicao> BuscarPelaConta(Conta conta);
        IEnumerable<Eleicao> BuscarPeloUsuario(Usuario usuario);
        Eleitor BuscarEleitor(Eleicao eleicao, int id);
        int QtdaEleitores(Eleicao eleicao);
        int QtdaVotos(Eleicao eleicao);
    }
}