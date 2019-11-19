using System;

namespace Cipa.WebApi.ViewModels {

    public class InscricaoViewModel {
        public int Id { get; set; }
        public string StatusAprovacao { get; set; }
        public int EleitorId { get; set; }
        public string Objetivos { get; set; }
        public string EmailAprovador { get; set; }
        public string NomeAprovador { get; set; }
        public DateTime? HorarioAprovacao { get; set; }
        public EleitorViewModel Eleitor { get; set; }

    }
}