using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cipa.Domain.Entities;

namespace Cipa.Application.Interfaces
{
    public interface IArquivoAppService: IAppServiceBase<Arquivo>
    {
        void ExcluiArquivos(DependencyFileType dependency, int id);
        IEnumerable<Arquivo> BuscaArquivos(DependencyFileType dependency, int id);
        Task<Arquivo> SalvarArquivo(Stream file, DependencyFileType dependencyType, int dependencyId, string emailUsuario, string nomeUsuario, string nomeArquivo, string contentType);
        string GerarURLTemporaria(int id);
    }
}