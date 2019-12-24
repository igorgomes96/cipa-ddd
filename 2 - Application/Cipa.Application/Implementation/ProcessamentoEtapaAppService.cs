using System.Linq;
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Factories.Interfaces;
using Cipa.Domain.Helpers;
using Cipa.Application.Repositories;

namespace Cipa.Application
{
    public class ProcessamentoEtapaAppService : AppServiceBase<ProcessamentoEtapa>, IProcessamentoEtapaAppService
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IFormatadorEmailServiceFactory _formatadorEmail;
        public ProcessamentoEtapaAppService(IUnitOfWork unitOfWork, EmailConfiguration emailConfiguration, IFormatadorEmailServiceFactory formatadorEmail) : base(unitOfWork, unitOfWork.ProcessamentoEtapaRepository)
        {
            _emailConfiguration = emailConfiguration;
            _formatadorEmail = formatadorEmail;
        }

        public void ProcessarEtapasPendentes()
        {
            var processamentos = (_repositoryBase as IProcessamentoEtapaRepository).BuscarProcessamentosPendentes().ToList();

            foreach (var processamento in processamentos)
            {
                processamento.IniciarProcessamento();
                base.Atualizar(processamento);
                var emails = processamento.RealizarProcessamentoGerarEmails(_emailConfiguration, _formatadorEmail);
                foreach (var email in emails)
                    _unitOfWork.EmailRepository.Adicionar(email);
                base.Atualizar(processamento);                    
            }
        }
    }
}