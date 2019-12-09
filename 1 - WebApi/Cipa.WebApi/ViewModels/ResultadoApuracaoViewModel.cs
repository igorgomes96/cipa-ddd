using System.Collections.Generic;

namespace Cipa.WebApi.ViewModels
{
    public class ResultadoApuracaoViewModel
    {
        public IEnumerable<ApuracaoViewModel> Efetivos { get; set; }
        public IEnumerable<ApuracaoViewModel> Suplentes { get; set; }
        public IEnumerable<ApuracaoViewModel> NaoEleitos { get; set; }
    }
}
