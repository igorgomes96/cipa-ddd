using Cipa.Domain.Interfaces.Repositories;
using Cipa.Domain.Entities;
using Cipa.Infra.Data.Context;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Cipa.Infra.Data.Repositories
{
    public class UsuarioRepository : RepositoryBase<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(CipaContext db) : base(db) { }

        private IQueryable<Usuario> QueryUsuario()
        {
            return DbSet.Include(u => u.Conta);
        }

        public override Usuario BuscarPeloId(int id)
        {
            return QueryUsuario().SingleOrDefault(u => u.Id == id);
        }

        public Usuario BuscarPeloIdCarregarAgregadoConta(int id)
        {
            return QueryUsuario()
                .Include(u => u.Conta)
                    .ThenInclude(c => c.EtapasPadroes)
                .SingleOrDefault(u => u.Id == id);
        }

        public Usuario BuscarUsuario(string email, string senha)
        {
            return QueryUsuario().SingleOrDefault(u => u.Email == email && u.Senha == senha);
        }

        public Usuario BuscarUsuario(string email)
        {
            return QueryUsuario().SingleOrDefault(u => u.Email == email);
        }
    }
}