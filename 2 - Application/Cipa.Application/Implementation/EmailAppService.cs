using System.Linq;
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;
using Cipa.Application.Repositories;
using Cipa.Domain.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

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
                Thread.Sleep(TimeSpan.FromMilliseconds(100));  // O SES da AWS tem o limite de 14 envios por segundo.
            }
            Task.WaitAll(tasks.ToArray());
            foreach (var email in emails) {
                base.Atualizar(email);
            }
        }
    }
}