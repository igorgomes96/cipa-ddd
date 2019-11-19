

namespace Cipa.Domain.Entities {
    public class LimiteDimensionamento : Entity<int> {
        public int Limite { get; set; }
        public int IntervaloAcrescimo { get; set; }
        public int AcrescimoEfetivos { get; set; }
        public int AcrescimoSuplentes { get; set; }
    }
}