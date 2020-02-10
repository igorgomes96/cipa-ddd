using Cipa.Domain.Entities;
using System.Collections.Generic;

namespace Cipa.Application.Repositories
{
    public interface IEstabelecimentoRepository : IRepositoryBase<Estabelecimento>
    {
        IEnumerable<Estabelecimento> BuscarEstabelecimentosPorConta(int contaId);
        IEnumerable<Estabelecimento> BuscarEstabelecimentosPorEmpresa(int empresaId);
    }
}