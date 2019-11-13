using System;
using System.Collections.Generic;


namespace Cipa.Domain.Entities
{
    public class Conta
    {
        public int Id { get; set; }
        public int? PlanoId { get; set; }
        public bool Ativa { get; set; }
        public int QtdaEstabelecimentos { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public DateTime DataCadastro { get; set; }

        public virtual Plano Plano { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; } = new List<Usuario>();
        public virtual ICollection<Empresa> Empresas { get; } = new List<Empresa>();
        public virtual ICollection<EtapaPadraoConta> EtapasPadroes { get; } = new List<EtapaPadraoConta>();
    }
}
