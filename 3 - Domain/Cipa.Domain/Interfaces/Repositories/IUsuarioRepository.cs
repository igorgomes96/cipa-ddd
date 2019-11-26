using Cipa.Domain.Entities;

namespace Cipa.Domain.Interfaces.Repositories {
    public interface IUsuarioRepository: IRepositoryBase<Usuario>
    {
        // Usuario BuscarPeloIdCarregarAgregadoConta(int id);
        Usuario BuscarUsuario(string email);
        Usuario BuscarUsuario(string email, string senha);
        
    }
}