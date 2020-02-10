using Cipa.Domain.Enums;

namespace Cipa.WebApi.ViewModels
{
    public class EtapaPadraoContaViewModel : EtapaBaseViewModel
    {
        public int ContaId { get; set; }
        public ECodigoEtapaObrigatoria? EtapaObrigatoriaId { get; set; }
        public int DuracaoPadrao { get; set; }
    }
}