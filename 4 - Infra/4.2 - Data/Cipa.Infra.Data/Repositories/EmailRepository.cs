using System.Collections.Generic;
using System.Linq;
using Cipa.Domain.Entities;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Infra.Data.Context;

namespace Cipa.Infra.Data.Repositories
{
    public class EmailRepository : RepositoryBase<Email>, IEmailRepository
    {
        public EmailRepository(CipaContext db) : base(db) { }

        public IEnumerable<Email> BuscarEmailsPendentes()
        {
            return BuscarTodos().Where(e => e.StatusEnvio == StatusEnvio.Pendente);
        }
    }
}