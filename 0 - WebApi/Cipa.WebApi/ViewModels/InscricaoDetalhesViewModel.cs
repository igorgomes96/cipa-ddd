using System;
using System.Collections.Generic;

namespace Cipa.WebApi.ViewModels {

    public class InscricaoDetalhesViewModel: InscricaoViewModel {
        public ICollection<ReprovacaoViewModel> Reprovacoes { get; set; }
    }
}