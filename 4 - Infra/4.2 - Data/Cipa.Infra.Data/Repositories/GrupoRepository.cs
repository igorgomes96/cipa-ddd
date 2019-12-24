using Cipa.Domain.Entities;
using Cipa.Application.Repositories;
using Cipa.Infra.Data.Context;

namespace Cipa.Infra.Data.Repositories
{
    public class GrupoRepository : RepositoryBase<Grupo>, IGrupoRepository
    {
        public GrupoRepository(CipaContext db) : base(db) { }

    }
}