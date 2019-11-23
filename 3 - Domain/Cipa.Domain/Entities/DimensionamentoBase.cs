using System;
using System.Collections.Generic;

namespace Cipa.Domain.Entities
{
    public class DimensionamentoBase : ValueObject
    {
        protected DimensionamentoBase(int maximo, int minimo, int qtdaEfetivos, int qtdaSuplentes)
        {
            Minimo = minimo;
            Maximo = maximo;
            QtdaEfetivos = qtdaEfetivos;
            QtdaSuplentes = qtdaSuplentes;
        }
        public int Id { get; set; }
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

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Minimo;
            yield return Maximo;
            yield return QtdaEfetivos;
            yield return QtdaSuplentes;
        }
    }
}