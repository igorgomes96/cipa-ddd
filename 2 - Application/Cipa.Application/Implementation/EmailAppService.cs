using System.Linq;
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;
using Cipa.Application.Repositories;
using Cipa.Domain.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cipa.Application
{
    public class EmailAppService : AppServiceBase<Email>, IEmailAppService
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IEmailSender _emailSender;
        public EmailAppService(IUnitOfWork unitOfWork, EmailConfiguration emailConfiguration, IEmailSender emailSender) : base(unitOfWork, unitOfWork.EmailRepository)
        {
            _emailConfiguration = emailConfiguration;
            _emailSender = emailSender;
        }

        public void EnviarEmailsBackground()
        {
            var emails = ((IEmailRepository)_repositoryBase).BuscarEmailsPendentes().ToList();
            
            foreach (var email in emails)
            {
                email.IniciarProcessoEnvio();
                base.Atualizar(email);
            }

            var tasks = new List<Task>();
            foreach (var email in emails)
            {
                tasks.Add(_emailSender.Send(email));
                // email.Enviar(_emailConfiguration);
                base.Atualizar(email);
            }
            Task.WaitAll(tasks.ToArray());
        }
    }
}