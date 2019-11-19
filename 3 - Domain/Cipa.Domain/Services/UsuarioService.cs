using Cipa.Domain.Entities;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Domain.Interfaces.Services;

namespace Cipa.Domain.Services {
    public class UsuarioService: ServiceBase<Usuario>, IUsuarioService {

        private readonly IUsuarioRepository _usuarioRepository;
        public UsuarioService(IUsuarioRepository usuarioRepository): base(usuarioRepository) {
            _usuarioRepository = usuarioRepository;
        }

        public Usuario BuscarUsuario(string email, string senha)
        {
            return _usuarioRepository.BuscarUsuario(email, senha);
        }

        public Usuario BuscarUsuario(string email)
        {
            return _usuarioRepository.BuscarUsuario(email);
        }
    }
}