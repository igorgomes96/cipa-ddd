using System.Collections.Generic;
using System.Linq;
using Cipa.Domain.Entities;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Infra.Data.Context;

namespace Cipa.Infra.Data.Repositories
{
    public class ArquivoRepository : RepositoryBase<Arquivo>, IArquivoRepository
    {
        public ArquivoRepository(CipaContext db) : base(db)
        {
        }

        public IEnumerable<Arquivo> BuscaArquivos(DependencyFileType dependency, int id) =>
            _db.Arquivos.Where(a => a.DependencyId == id && a.DependencyType == dependency);

    }
}