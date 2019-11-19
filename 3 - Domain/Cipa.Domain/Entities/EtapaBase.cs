namespace Cipa.Domain.Entities
{
    public class EtapaBase<TKey> : Entity<TKey>
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int Ordem { get; set; }
    }
}