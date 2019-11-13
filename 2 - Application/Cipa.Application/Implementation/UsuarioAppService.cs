using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Interfaces.Services;
using Cipa.Domain.Services;

namespace Cipa.Application {
    public class UsuarioAppService: AppServiceBase<Usuario>, IUsuarioAppService {
        private readonly IUsuarioService _usuarioService;
        public UsuarioAppService(IUsuarioService usuarioService): base(usuarioService) {
            _usuarioService = usuarioService;
        }

        public Usuario BuscarUsuario(string email, string senha)
        {
            return _usuarioService.BuscarUsuario(email, senha);
        }
    }
}