using Cipa.Domain.Entities;

namespace Cipa.Application.Repositories
{
    public interface IImportacaoRepository : IRepositoryBase<Importacao>
    {
        Importacao BuscarPrimeiraImportacaoPendenteDaFila();
    }
}