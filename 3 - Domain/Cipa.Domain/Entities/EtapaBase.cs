using System.Collections.Generic;

namespace Cipa.Domain.Entities
{
    public class EtapaBase : ValueObject
    {
        public EtapaBase(string nome, string descricao, int ordem)
        {
            Nome = nome;
            Descricao = descricao;
            Ordem = ordem;
        }
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int Ordem { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Ordem;
        }
    }
}