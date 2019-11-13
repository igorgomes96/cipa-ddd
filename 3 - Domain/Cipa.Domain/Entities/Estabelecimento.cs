using System;
using System.Collections.Generic;

namespace Cipa.Domain.Entities
{
    public class Estabelecimento
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public string Cidade { get; set; }
        public string Endereco { get; set; }
        public string Descricao { get; set; }
        public int? GrupoId { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }

        public virtual Empresa Empresa { get; set; }
        public virtual Grupo Grupo { get; set; }


    }
}