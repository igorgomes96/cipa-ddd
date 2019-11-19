using System;

namespace Cipa.Domain.Entities {
    public class Voto: Entity<int> {
        public int EleitorId { get; set; }
        public int EleicaoId { get; set; }
        public string IP { get; set; }
        public DateTime DataCadastro { get; set; }

        public virtual Eleicao Eleicao { get; set; }
        public virtual Eleitor Eleitor { get; set; }
    }
}