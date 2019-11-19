using Cipa.Domain.Entities;

namespace Cipa.Domain.Interfaces.Services {
    public interface IUsuarioService: IServiceBase<Usuario>
    {
        Usuario BuscarUsuario(string email);
        Usuario BuscarUsuario(string email, string senha);
    }
}