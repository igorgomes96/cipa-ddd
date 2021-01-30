using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cipa.Domain.Entities;

namespace Cipa.Application.Repositories
{
    public interface IArquivoRepository : IRepositoryBase<Arquivo>
    {
        Task<Arquivo> Adicionar(Arquivo obj, Stream file);
        Task<string> RealizarUpload(Stream file, string fileKey);
        void ExluirArquivoNuvem(string arquivo);
        IEnumerable<Arquivo> BuscaArquivos(DependencyFileType dependency, int id);
    }
}