using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cipa.Application.Helpers;
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Interfaces.Repositories;

namespace Cipa.Application
{
    public class ArquivoAppService : AppServiceBase<Arquivo>, IArquivoAppService
    {
        private const string PATH_DOCUMENTOS = @"StaticFiles\Documentos\";
        public ArquivoAppService(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.ArquivoRepository)
        {

        }

        public IEnumerable<Arquivo> BuscaArquivos(DependencyFileType dependency, int id) =>
            (_repositoryBase as IArquivoRepository).BuscaArquivos(dependency, id);

        public override Arquivo Excluir(int id)
        {
            var arquivo = base.BuscarPeloId(id);
            if (arquivo == null) throw new NotFoundException("Arquivo não encontrado.");
            return Excluir(arquivo);
        }

        public override Arquivo Excluir(Arquivo arquivo)
        {
            if (!string.IsNullOrWhiteSpace(arquivo.Path))
            {
                string file = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), arquivo.Path);
                if (File.Exists(file))
                    File.Delete(file);
            }
            return base.Excluir(arquivo);
        }

        public void ExcluiArquivos(DependencyFileType dependency, int id)
        {
            var arquivos = (_repositoryBase as IArquivoRepository).BuscaArquivos(dependency, id);
            foreach (var arquivo in arquivos)
                Excluir(arquivo);
        }

        private string GetRelativePath(DependencyFileType dependencyType, int dependencyId)
        {
            return $@"{PATH_DOCUMENTOS}{dependencyType.ToString().ToLower()}\{dependencyId.ToString()}";
        }


        public Arquivo SalvarArquivo(DependencyFileType dependencyType, int dependencyId, string emailUsuario, string nomeUsuario, byte[] arquivo, string nomeArquivo, string contentType)
        {
            string relativePath = GetRelativePath(dependencyType, dependencyId);
            string path = FileSystemHelpers.GetAbsolutePath(relativePath);

            Directory.CreateDirectory(path);
            string file = FileSystemHelpers.GetRelativeFileName(path, nomeArquivo);
            File.WriteAllBytes(file, arquivo);

            var novoArquivo = new Arquivo(
                Path.Combine(relativePath, Path.GetFileName(file)), nomeArquivo, new FileInfo(file).Length,
                contentType, emailUsuario, nomeArquivo, dependencyType, dependencyId
            );

            return base.Adicionar(novoArquivo);
        }
    }
}