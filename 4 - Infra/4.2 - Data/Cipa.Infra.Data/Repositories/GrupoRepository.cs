using Cipa.Domain.Interfaces.Repositories;
using Cipa.Domain.Entities;
using Cipa.Infra.Data.Context;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Cipa.Infra.Data.Repositories
{
    public class GrupoRepository : RepositoryBase<Grupo>, IGrupoRepository
    {
        public GrupoRepository(CipaContext db) : base(db) { }
        
        private IQueryable<Grupo> QueryGrupo()  {
            return DbSet
                .Include(g => g.LimiteDimensionamento)
                .Include(g => g.Dimensionamentos);
        }

        public override IEnumerable<Grupo> BuscarTodos() => QueryGrupo();

        public override Grupo BuscarPeloId(int id) => QueryGrupo().SingleOrDefault(g => g.Id == id);
    }
}