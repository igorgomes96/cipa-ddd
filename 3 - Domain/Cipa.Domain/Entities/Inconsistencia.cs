namespace Cipa.Domain.Entities
{
    public class Inconsistencia : Entity<int>
    {
        public Inconsistencia(string coluna, int linha, string mensagem)
        {
            Coluna = coluna;
            Linha = linha;
            Mensagem = mensagem;
        }

        public string Coluna { get; private set; }
        public int Linha { get; private set; }
        public string Mensagem { get; private set; }
        public int ImportacaoId { get; private set; }

        public virtual Importacao Importacao { get; private set; }
    }
}