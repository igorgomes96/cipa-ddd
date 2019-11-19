using System;


namespace Cipa.Domain.Entities {
    public class Reprovacao: Entity<int> {
        public int InscricaoId { get; set; }
        public string MotivoReprovacao { get; set; }
        public DateTime DataCadastro { get; set; }
        public string EmailAprovador { get; set; }
        public string NomeAprovador { get; set; }

        public virtual Inscricao Inscricao { get; set; }
    }
}