using System;

namespace Cipa.Domain.Entities
{
    public class DimensionamentoBase : Entity<int>
    {
        protected DimensionamentoBase(int maximo, int minimo, int qtdaEfetivos, int qtdaSuplentes)
        {
            Minimo = minimo;
            Maximo = maximo;
            QtdaEfetivos = qtdaEfetivos;
            QtdaSuplentes = qtdaSuplentes;
        }
        public int Minimo { get; internal set; }
        public int Maximo { get; internal set; }
        public int QtdaEfetivos { get; internal set; }
        public int QtdaSuplentes { get; internal set; }

        public int TotalCipeiros
        {
            get
            {
                return QtdaEfetivos + QtdaSuplentes;
            }
        }

    }
}