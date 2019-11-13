namespace Cipa.Domain.Entities {
    public class Inconsistencia {
        public int Id { get; set; }
        public string Coluna { get; set; }
        public int Linha { get; set; }
        public string Mensagem { get; set; }
        public int ImportacaoId { get; set; }

        public virtual Importacao Importacao { get; set; }
    }
}