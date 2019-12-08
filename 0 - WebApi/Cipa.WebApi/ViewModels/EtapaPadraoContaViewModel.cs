using Cipa.Domain.Helpers;

namespace Cipa.WebApi.ViewModels
{
    public class EtapaPadraoContaViewModel : EtapaBaseViewModel
    {
        public int ContaId { get; set; }
        public CodigoEtapaObrigatoria? EtapaObrigatoriaId { get; set; }
        public int DuracaoPadrao { get; set; }
    }
}