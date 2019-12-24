using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Application.Repositories
{
    public interface IEmailRepository : IRepositoryBase<Email>
    {
        IEnumerable<Email> BuscarEmailsPendentes();
    }
}