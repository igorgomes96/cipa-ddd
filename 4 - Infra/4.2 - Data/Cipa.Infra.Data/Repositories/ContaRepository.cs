using Cipa.Domain.Interfaces.Repositories;
using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Cipa.Infra.Data.Context;

namespace Cipa.Infra.Data.Repositories
{
    public class ContaRepository : RepositoryBase<Conta>, IContaRepository
    {
        public ContaRepository(CipaContext db) : base(db) { }

        public override Conta BuscarPeloId(int id) {
            return DbSet.Include(c => c.EtapasPadroes).SingleOrDefault(c => c.Id == id);
        }
    }
}