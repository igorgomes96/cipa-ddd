using System;
using System.ComponentModel.DataAnnotations;

namespace Cipa.WebApi.ViewModels {
    public class ReprovacaoViewModel
    {
        public int Id { get; set; }
        public int CandidatoId { get; set; }
        [Required(ErrorMessage = "O motivo da reprovação da inscrição deve ser informado.")]
        [StringLength(255, ErrorMessage = "O motivo da reprovação pode conter no máximo {1} caracteres.")]
        public string MotivoReprovacao { get; set; }
        public DateTime Horario { get; set; }
        public string EmailAprovador { get; set; }
        public string NomeAprovador { get; set; }
    }
}