using System;
using System.Collections.Generic;

namespace Cipa.WebApi.ViewModels
{
    public class ContaViewModel
    {
        public int Id { get; set; }
        public int? PlanoId { get; set; }
        public bool Ativa { get; set; }
        public int QtdaEstabelecimentos { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        public virtual ICollection<UsuarioViewModel> Usuarios { get; set; }

    }
}
