using System;
using System.Collections.Generic;
using Cipa.Domain.Helpers;

namespace Cipa.Domain.Entities
{
    public class Empresa: Entity<int>
    {
        public string RazaoSocial { get; set; }
        public string Cnpj { get; set; }
        public string InformacoesGerais { get; set; }
        public int ContaId { get; set; }
        public bool Ativa { get; set; }
        public DateTime DataCadastro { get; set; }

        public virtual Conta Conta { get; private set; }
        public virtual ICollection<Estabelecimento> Estabelecimentos { get; } = new List<Estabelecimento>();

        public string CnpjFormatado
        {
            get
            {
                return Util.FormataCNPJ(Cnpj);
            }
        }

    }
}