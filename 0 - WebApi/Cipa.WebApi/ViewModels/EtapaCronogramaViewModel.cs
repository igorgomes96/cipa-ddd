using System;
using Cipa.Domain.Helpers;

namespace Cipa.WebApi.ViewModels {

    public class EtapaCronogramaViewModel: EtapaBaseViewModel
    {
        public int EleicaoId { get; set; }
        public DateTime DataPrevista { get; set; }
        public DateTime? DataRealizada { get; set; }
        public CodigoEtapaObrigatoria? EtapaObrigatoriaId { get; set; }
        public string PosicaoEtapa { get; set; }
        public string ErroMudancaEtapa { get; set; }
        public bool PossuiTemplates { get; set; }

    }
}
