using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Domain.Interfaces.Repositories
{
    public interface IEmailRepository : IRepositoryBase<Email>
    {
        IEnumerable<Email> BuscarEmailsPendentes();
    }
}