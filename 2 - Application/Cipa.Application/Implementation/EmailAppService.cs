using System.Linq;
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;
using Cipa.Domain.Interfaces.Repositories;

namespace Cipa.Application
{
    public class EmailAppService : AppServiceBase<Email>, IEmailAppService
    {
        private readonly EmailConfiguration _emailConfiguration;
        public EmailAppService(IUnitOfWork unitOfWork, EmailConfiguration emailConfiguration) : base(unitOfWork, unitOfWork.EmailRepository)
        {
            _emailConfiguration = emailConfiguration;
        }

        public void EnviarEmailsBackground()
        {
            var emails = ((IEmailRepository)_repositoryBase).BuscarEmailsPendentes().ToList();
            
            foreach (var email in emails)
            {
                email.IniciarProcessoEnvio();
                base.Atualizar(email);
            }

            foreach (var email in emails)
            {
                email.Enviar(_emailConfiguration);
                base.Atualizar(email);
            }
        }
    }
}