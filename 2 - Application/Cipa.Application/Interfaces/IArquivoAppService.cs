using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Application.Interfaces
{
    public interface IArquivoAppService: IAppServiceBase<Arquivo>
    {
        void ExcluiArquivos(DependencyFileType dependency, int id);
        IEnumerable<Arquivo> BuscaArquivos(DependencyFileType dependency, int id);
        Arquivo SalvarArquivo(DependencyFileType dependencyType, int dependencyId, string emailUsuario, string nomeUsuario, byte[] arquivo, string nomeArquivo, string contentType);
    }
}