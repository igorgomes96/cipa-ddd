using System;

namespace Cipa.WebApi.ViewModels
{
    public class EleicaoDetalheViewModel : EleicaoViewModel
    {
        public bool InscricoesFinalizadas { get; set; }
        public bool VotacaoFinalizada { get; set; }
        public DateTime InicioInscricao { get; set; }
        public DateTime TerminoInscricao { get; set; }
        public DateTime InicioVotacao { get; set; }
        public DateTime TerminoVotacao { get; set; }
        public bool UsuarioEleitor { get; set; }
    }
}