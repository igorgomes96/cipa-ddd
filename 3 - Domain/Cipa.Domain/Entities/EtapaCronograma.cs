using System;
using Cipa.Domain.Helpers;

namespace Cipa.Domain.Entities
{
    public class EtapaCronograma : EtapaBase<int>
    {
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
            var compareTo = obj as EtapaCronograma;

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return Id.Equals(compareTo.Id);
        }

        public static bool operator ==(EtapaCronograma a, EtapaCronograma b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(EtapaCronograma a, EtapaCronograma b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().GetHashCode() * 907) + Id.GetHashCode();
        }

        public override string ToString()
        {
            return GetType().Name + " [Id=" + Id + "]";
        }

    }
}
