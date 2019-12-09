using System;

namespace Cipa.WebApi.ViewModels
{
    public class ApuracaoViewModel
    {
        public int EleicaoId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Matricula { get; set; }
        public string Cargo { get; set; }
        public string Area { get; set; }
        public int Votos { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public DateTime? DataNascimento { get; set; }
        public int? InscricaoId { get; set; }
        public string ResultadoApuracao { get; set; }
    }
}