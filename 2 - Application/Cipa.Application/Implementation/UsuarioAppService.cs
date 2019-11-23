using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Interfaces.Repositories;

namespace Cipa.Application
{
    public class UsuarioAppService : AppServiceBase<Usuario>, IUsuarioAppService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UsuarioAppService(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.UsuarioRepository)
        {
            _unitOfWork = unitOfWork;
        }

        public Usuario BuscarUsuario(string email, string senha)
        {
            return _unitOfWork.UsuarioRepository.BuscarUsuario(email, senha);
        }
    }
}