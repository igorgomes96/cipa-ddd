using Cipa.Domain.Entities;

namespace Cipa.Domain.Interfaces.Repositories {
    public interface IUsuarioRepository: IRepositoryBase<Usuario>
    {
        Usuario BuscarUsuario(string email, string senha);
    }
}