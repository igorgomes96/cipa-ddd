using System;

namespace Cipa.Domain.Entities
{
    public class DimensionamentoBase
    {
        public int Id { get; set; }
        public int Minimo { get; set; }
        public int Maximo { get; set; }
        public int QtdaEfetivos { get; set; }
        public int QtdaSuplentes { get; set; }

        public int TotalCipeiros
        {
            get
            {
                return QtdaEfetivos + QtdaSuplentes;
            }
        }

    }
}