using Cipa.Domain.Entities;

namespace Cipa.Domain.Interfaces.Repositories
{
    public interface IImportacaoRepository : IRepositoryBase<Importacao>
    {
        Importacao BuscarPrimeiraImportacaoPendenteDaFila();
    }
}