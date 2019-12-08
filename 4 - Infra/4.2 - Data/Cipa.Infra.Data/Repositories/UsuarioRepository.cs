using Cipa.Domain.Entities;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Infra.Data.Context;
using System.Collections.Generic;
using System.Linq;

namespace Cipa.Infra.Data.Repositories
{
    public class UsuarioRepository : RepositoryBase<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(CipaContext db) : base(db) { }

        public Usuario BuscarUsuario(string email, string senha)
        {
            return DbSet.SingleOrDefault(u => u.Email == email && u.Senha == senha);
        }

        public Usuario BuscarUsuario(string email)
        {
            return DbSet.SingleOrDefault(u => u.Email == email);
        }

        public IEnumerable<Usuario> BuscarUsuariosPelaConta(int contaId)
        {
            return DbSet.Where(u => u.ContaId == contaId);
        }
    }
}