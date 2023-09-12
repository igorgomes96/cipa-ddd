using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Application.Repositories;

namespace Cipa.Application.Implementation
{
    public class GrupoAppService : AppServiceBase<Grupo>, IGrupoAppService
    {
        public GrupoAppService(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.GrupoRepository)
        {
        }
    }
}