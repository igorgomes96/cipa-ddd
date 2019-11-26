using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Interfaces.Repositories;

namespace Cipa.Application {
    public class GrupoAppService : AppServiceBase<Grupo>, IGrupoAppService
    {
        public GrupoAppService(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.GrupoRepository)
        {
        }
    }
}