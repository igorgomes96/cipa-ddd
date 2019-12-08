using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cipa.Domain.Entities
{
    public class Empresa : Entity<int>
    {
        public Empresa(string razaoSocial, string cnpj)
        {
            RazaoSocial = razaoSocial;
            Cnpj = cnpj;
            Ativa = true;
        }
        public string RazaoSocial { get; set; }
        public string Cnpj { get; set; }
        public string InformacoesGerais { get; set; }
        public int ContaId { get; set; }
        public bool Ativa { get; private set; }
        public DateTime DataCadastro { get; private set; }

        public virtual Conta Conta { get; private set; }
        public virtual ICollection<Estabelecimento> Estabelecimentos { get; } = new List<Estabelecimento>();

        public string CnpjFormatado
        {
            get => Util.FormataCNPJ(Cnpj);
        }

        public List<Estabelecimento> EstabelecimentosAtivos
        {
            get => Estabelecimentos.Where(e => e.Ativo).ToList();
        }

        public void Inativar()
        {
            if (EstabelecimentosAtivos.Any())
                throw new CustomException("Há estabelecimentos cadastrados para essa empresa.");
            Ativa = false;
        }

    }
}