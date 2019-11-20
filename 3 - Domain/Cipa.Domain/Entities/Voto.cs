using System;

namespace Cipa.Domain.Entities
{
    public class Voto : Entity<int>
    {
        public Voto() { }
        public Voto(Eleitor eleitor, string ip)
        {
            EleitorId = eleitor.Id;
            Eleitor = eleitor;
            EleicaoId = eleitor.EleicaoId;
            IP = ip;
        }
        public int EleitorId { get; set; }
        public int EleicaoId { get; set; }
        public string IP { get; set; }
        public DateTime DataCadastro { get; set; }

        public virtual Eleicao Eleicao { get; set; }
        public virtual Eleitor Eleitor { get; set; }

    }
}