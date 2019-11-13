using System;
using Cipa.Domain.Helpers;

namespace Cipa.Domain.Entities
{
    public class EtapaCronograma : EtapaBase
    {
        public int Id { get; set; }
        public int EleicaoId { get; set; }
        public DateTime DataPrevista { get; set; }
        public DateTime? DataRealizada { get; set; }
        public CodigoEtapaObrigatoria? EtapaObrigatoriaId { get; set; }
        public PosicaoEtapa PosicaoEtapa { get; set; }
        public string ErroMudancaEtapa { get; set; }

        public virtual EtapaObrigatoria EtapaObrigatoria { get; set; }
        public virtual Eleicao Eleicao { get; set; }
        
        public DateTime Data
        {
            get
            {
                return DataRealizada ?? DataPrevista;
            }
        }

        public override bool Equals(object obj)
        {
            return (base.Equals(obj) || ((EtapaCronograma)obj).Id == Id);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Id.GetHashCode();
            hash = (hash * 7) + Id.GetHashCode();
            return hash;
        }

    }
}
