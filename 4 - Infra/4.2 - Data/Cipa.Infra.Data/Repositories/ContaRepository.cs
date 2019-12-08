using Cipa.Domain.Entities;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Infra.Data.Context;

namespace Cipa.Infra.Data.Repositories
{
    public class ContaRepository : RepositoryBase<Conta>, IContaRepository
    {
        public ContaRepository(CipaContext db) : base(db) { }

    }
}