using System.Collections.Generic;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Domain.Interfaces.Services;

namespace Cipa.Domain.Services {
    public class ContaService : ServiceBase<Conta>, IContaService
    {
        public ContaService(IContaRepository contaRepository): base(contaRepository) { }

    }
}