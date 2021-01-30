using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cipa.Domain.Entities;

namespace Cipa.Application.Repositories
{
    public interface IArquivoRepository : IRepositoryBase<Arquivo>
    {
        Task<Arquivo> Adicionar(Arquivo obj, Stream file);
        IEnumerable<Arquivo> BuscaArquivos(DependencyFileType dependency, int id);
        string GerarURLTemporaria(int id);
    }
}