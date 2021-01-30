using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Application.Repositories;
using System.Threading.Tasks;
using System.Linq;

namespace Cipa.Application
{
    public class ArquivoAppService : AppServiceBase<Arquivo>, IArquivoAppService
    {
        public ArquivoAppService(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.ArquivoRepository)
        { }

        public IEnumerable<Arquivo> BuscaArquivos(DependencyFileType dependency, int id) =>
            (_repositoryBase as IArquivoRepository).BuscaArquivos(dependency, id);


        public void ExcluiArquivos(DependencyFileType dependency, int id)
        {
            var arquivos = (_repositoryBase as IArquivoRepository).BuscaArquivos(dependency, id);
            foreach (var arquivo in arquivos)
                Excluir(arquivo);
        }

        public async Task<Arquivo> SalvarArquivo(Stream file, DependencyFileType dependencyType, int dependencyId, string emailUsuario, string nomeUsuario, string nomeArquivo, string contentType)
        {
            var arquivosExistentes = _unitOfWork.ArquivoRepository.BuscaArquivos(dependencyType, dependencyId);
            if (arquivosExistentes.Any(a => a.Nome == nomeArquivo))
                throw new DuplicatedException("Já há um arquivo salvo com esse nome.");
                
            var novoArquivo = new Arquivo(
                nomeArquivo, file.Length, contentType, emailUsuario,
                nomeUsuario, dependencyType, dependencyId
            );
            var arquivoSalvo = await _unitOfWork.ArquivoRepository.Adicionar(novoArquivo, file);
            _unitOfWork.Commit();
            return arquivoSalvo;
        }

    }
}