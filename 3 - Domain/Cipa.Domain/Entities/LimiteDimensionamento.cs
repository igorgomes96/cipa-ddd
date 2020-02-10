

using System.Collections.Generic;

namespace Cipa.Domain.Entities
{
    public class LimiteDimensionamento : ValueObject
    {

        public LimiteDimensionamento() { }
        public LimiteDimensionamento(int limite, int intervaloAcrescimo, int acrescimoEfetivos, int acrescimoSuplentes)
        {
            Limite = limite;
            IntervaloAcrescimo = intervaloAcrescimo;
            AcrescimoEfetivos = acrescimoEfetivos;
            AcrescimoSuplentes = acrescimoSuplentes;
        }
        public int Id { get; set; }
        public int Limite { get; private set; }
        public int IntervaloAcrescimo { get; private set; }
        public int AcrescimoEfetivos { get; private set; }
        public int AcrescimoSuplentes { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
        }
    }
}