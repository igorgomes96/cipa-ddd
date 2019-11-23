namespace Cipa.WebApi.ViewModels
{
    public class DimensionamentoViewModel
    {
        public int Minimo { get; set; }
        public int Maximo { get; set; }
        public int QtdaEfetivos { get; set; }
        public int QtdaSuplentes { get; set; }
        public int QtdaEleitores { get; set; }
        public int QtdaInscricoes { get; set; }
        public int QtdaInscricoesAprovadas { get; set; }
        public int QtdaInscricoesReprovadas { get; set; }
        public int QtdaInscricoesPendentes { get; set; }
        public int QtdaVotos { get; set; }
        public int QtdaMinimaVotos { get; set; }
    }
}