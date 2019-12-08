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
        public string Path { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataCriacao { get; set; }
        public string Nome { get; set; }
        public long Tamanho { get; set; }
        public string ContentType { get; set; }
        public string EmailUsuario { get; set; }
        public string NomeUsuario { get; set; }

        public DependencyFileType DependencyType { get; set; }
        public int DependencyId { get; set; }
    }
}
