using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Interfaces.Repositories;

namespace Cipa.Application
{
    public class UsuarioAppService : AppServiceBase<Usuario>, IUsuarioAppService
    {
        public UsuarioAppService(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.UsuarioRepository) { }

        public Usuario BuscarUsuario(string email, string senha) => 
            (_repositoryBase as IUsuarioRepository).BuscarUsuario(email, senha);
    }
}