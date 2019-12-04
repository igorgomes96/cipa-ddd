using System;
using System.ComponentModel.DataAnnotations;
using Cipa.Domain.Helpers;

namespace Cipa.WebApi.ViewModels {

    public class EtapaCronogramaViewModel: EtapaBaseViewModel
    {
        public int EleicaoId { get; set; }
        [Required(ErrorMessage = "A data prevista para essa etapa deve ser informada.")]
        public DateTime DataPrevista { get; set; }
        public DateTime? DataRealizada { get; set; }
        public CodigoEtapaObrigatoria? EtapaObrigatoriaId { get; set; }
        public string PosicaoEtapa { get; set; }
        public string ErroMudancaEtapa { get; set; }
        public bool PossuiTemplates { get; set; }

    }
}
