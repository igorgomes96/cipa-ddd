using Cipa.Domain.Entities;
using System.Collections.Generic;

namespace Cipa.Application.Repositories
{
    public interface IEleicaoRepository : IRepositoryBase<Eleicao>
    {
        // Eleicao BuscarPeloIdCarregarEleitores(int id);
        // Eleicao BuscarPeloIdCarregarVotos(int id);
        // Eleicao BuscarPeloIdCarregarTodoAgregado(int id);
        IEnumerable<Inscricao> BuscarInscricoes(int id, StatusInscricao? status = null);
        IEnumerable<Eleitor> BuscarEleitores(int id);
        IEnumerable<Voto> BuscarVotos(int id);
        IEnumerable<Eleicao> BuscarPelaConta(int contaId);
        IEnumerable<Eleicao> BuscarPeloUsuario(int usuarioId);

    }
}