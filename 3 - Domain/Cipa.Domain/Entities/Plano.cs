using System;

namespace Cipa.Domain.Entities
{
    public class Plano : Entity<int>
    {
        public string Descricao { get; set; }
        public decimal? ValorMes { get; set; }
        public decimal? ValorAno { get; set; }
        public int QtdaEstabelecimentos { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
