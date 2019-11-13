using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace Cipa.Domain.Entities
{
    public class Plano
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public decimal? ValorMes { get; set; }
        public decimal? ValorAno { get; set; }
        public int QtdaEstabelecimentos { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
