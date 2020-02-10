using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Application.Repositories
{
    public interface IArquivoRepository : IRepositoryBase<Arquivo>
    {
        IEnumerable<Arquivo> BuscaArquivos(DependencyFileType dependency, int id);
    }
}