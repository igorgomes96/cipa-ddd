using System;
using System.ComponentModel.DataAnnotations;

namespace Cipa.WebApi.ViewModels {

    public class InscricaoViewModel {
        public int Id { get; set; }
        public string StatusAprovacao { get; set; }
        public int EleitorId { get; set; }
        [StringLength(255, ErrorMessage = "Os objetivos devem conter no máximo {1} caracteres.")]
        public string Objetivos { get; set; }
        public string EmailAprovador { get; set; }
        public string NomeAprovador { get; set; }
        public DateTime? HorarioAprovacao { get; set; }
        public EleitorViewModel Eleitor { get; set; }

    }
}