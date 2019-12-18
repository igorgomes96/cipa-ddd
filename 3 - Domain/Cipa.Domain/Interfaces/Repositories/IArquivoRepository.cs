using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Domain.Interfaces.Repositories
{
    public interface IArquivoRepository : IRepositoryBase<Arquivo>
    {
        IEnumerable<Arquivo> BuscaArquivos(DependencyFileType dependency, int id);
    }
}