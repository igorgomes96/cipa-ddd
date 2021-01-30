using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Generic;
using System.Linq;
using Cipa.Application.Repositories;
using Cipa.Domain.Entities;
using Cipa.Infra.Data.Context;
using System.IO;
using Amazon.S3.Transfer;
using System.Threading.Tasks;
using Cipa.Domain.Exceptions;
using System;
using Microsoft.Extensions.Logging;

namespace Cipa.Infra.Data.Repositories
{
    public class ArquivoRepository : RepositoryBase<Arquivo>, IArquivoRepository
    {
        private const string BUCKET_NAME = "cipaonline";
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger<ArquivoRepository> _logger;

        public ArquivoRepository(CipaContext db, IAmazonS3 s3Client, ILogger<ArquivoRepository> logger) : base(db)
        {
            _s3Client = s3Client;
            _logger = logger;
        }

        private string GetRelativePath(DependencyFileType dependencyType, int dependencyId)
        {
            return $@"{dependencyType.ToString().ToLower()}/{dependencyId.ToString()}";
        }

        public override void Excluir(Arquivo obj)
        {
            base.Excluir(obj);
            ExluirArquivoNuvem(obj.Path);
        }

        public void ExluirArquivoNuvem(string arquivo)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = BUCKET_NAME,
                    Key = arquivo
                };
                _s3Client.DeleteObjectAsync(deleteObjectRequest).Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir arquivo do S3: " + ex.Message);
            }
        }
        public async Task<string> RealizarUpload(Stream file, string fileKey)
        {
            var newKey = Helpers.RemoverCaracteresEspeciais(fileKey);
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = file,
                Key = newKey,
                BucketName = BUCKET_NAME
            };

            var fileTransferUtility = new TransferUtility(_s3Client);
            await fileTransferUtility.UploadAsync(uploadRequest);
            return newKey;
        }

        public async Task<Arquivo> Adicionar(Arquivo obj, Stream file)
        {
            string relativePath = GetRelativePath(obj.DependencyType, obj.DependencyId);
            obj.Path = await RealizarUpload(file, $"{relativePath}/{obj.Nome}");
            return base.Adicionar(obj);
        }

        public string GerarURLTemporaria(int id)
        {
            var arquivo = base.BuscarPeloId(id);
            if (arquivo == null) throw new NotFoundException("Arquivo não encontrado.");
            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
            {
                BucketName = BUCKET_NAME,
                Key = arquivo.Path,
                Expires = DateTime.UtcNow.AddMinutes(15)
            };
            return _s3Client.GetPreSignedURL(request);
        }

        public IEnumerable<Arquivo> BuscaArquivos(DependencyFileType dependency, int id) =>
            _db.Arquivos.Where(a => a.DependencyId == id && a.DependencyType == dependency);

    }
}