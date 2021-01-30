using System;

namespace Cipa.Domain.Entities
{
    public enum DependencyFileType
    {
        TemplateCronograma,
        DocumentoCronograma,
        Importacao
    }
    public class Arquivo : Entity<int>
    {
        public Arquivo(string nome, long tamanho, string contentType, string emailUsuario, string nomeUsuario, DependencyFileType dependencyType, int dependencyId)
        {
            Nome = nome;
            Tamanho = tamanho;
            ContentType = contentType;
            EmailUsuario = emailUsuario;
            NomeUsuario = nomeUsuario;
            DependencyType = dependencyType;
            DependencyId = dependencyId;
        }

        public Arquivo(string path, string nome, long tamanho, string contentType, string emailUsuario, string nomeUsuario, DependencyFileType dependencyType, int dependencyId)
        {
            Path = path;
            Nome = nome;
            Tamanho = tamanho;
            ContentType = contentType;
            EmailUsuario = emailUsuario;
            NomeUsuario = nomeUsuario;
            DependencyType = dependencyType;
            DependencyId = dependencyId;
        }

        public string Path { get; set; }
        public string Nome { get; private set; }
        public long Tamanho { get; private set; }
        public string ContentType { get; private set; }
        public string EmailUsuario { get; private set; }
        public string NomeUsuario { get; private set; }
        public DependencyFileType DependencyType { get; private set; }
        public int DependencyId { get; private set; }
        public DateTime DataCadastro { get; private set; }

    }
}
