using Cipa.Domain.Helpers;
using System;
using System.Collections.Generic;

namespace Cipa.Domain.Entities
{
    public class EtapaCronograma : EtapaBase
    {
        public EtapaCronograma(string nome, string descricao, int ordem, int eleicaoId, DateTime dataPrevista, CodigoEtapaObrigatoria? etapaObrigatoriaId = null)
            : base(nome, descricao, ordem)
        {
            EleicaoId = eleicaoId;
            DataPrevista = dataPrevista;
            PosicaoEtapa = PosicaoEtapa.Futura;
            EtapaObrigatoriaId = etapaObrigatoriaId;
        }

        public int EleicaoId { get; private set; }
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

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return EleicaoId;
            yield return Ordem;
        }

        // public override bool Equals(object obj)
        // {
        //     return base.Equals(obj);
        // }

        // public override int GetHashCode()
        // {
        //     return base.GetHashCode();
        // }

        // public static bool operator == (EtapaCronograma a, EtapaCronograma b)
        // {
        //     return Equals(a, b);
        // }

        // public static bool operator != (EtapaCronograma a, EtapaCronograma b)
        // {
        //     return !(a == b);
        // }

    }
}
