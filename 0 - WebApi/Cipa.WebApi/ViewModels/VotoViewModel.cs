using System;

namespace Cipa.WebApi.ViewModels {
    public class VotoViewModel {
        public int Id { get; set; }
        public int EleitorId { get; set; }
        public string EleitorNome { get; set; }
        public string EleitorEmail { get; set; }
        public int EleicaoId { get; set; }
        public string IP { get; set; }
        public DateTime Horario { get; set; }
        
    }
}