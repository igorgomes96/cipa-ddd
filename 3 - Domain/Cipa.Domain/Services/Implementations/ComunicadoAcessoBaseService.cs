using System.Collections.Generic;
using Cipa.Domain.Entities;
using Cipa.Domain.Services.Interfaces;

namespace Cipa.Domain.Services.Implementations
{
    public abstract class ComunicadoAcessoBaseService : ComunicadoBaseService
    {

        public ComunicadoAcessoBaseService(Usuario usuario)
        {
            Usuario = usuario;
        }

        protected Usuario Usuario { get; }
    
    }
}