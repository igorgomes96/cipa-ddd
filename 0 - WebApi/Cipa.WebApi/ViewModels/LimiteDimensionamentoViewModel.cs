namespace Cipa.WebApi.ViewModels {
    public class LimiteDimensionamentoViewModel {
        public int Id { get; set; }
        public int Limite { get; set; }
        public int IntervaloAcrescimo { get; set; }
        public int AcrescimoEfetivos { get; set; }
        public int AcrescimoSuplentes { get; set; }
    }
}