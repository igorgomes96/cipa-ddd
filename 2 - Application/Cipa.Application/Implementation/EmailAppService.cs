using Cipa.Application.Interfaces;
using Cipa.Application.Repositories;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;
using Cipa.Domain.Services.Interfaces;
using System;
using System.Linq;
using System.Threading;

namespace Cipa.Application.Implementation
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

            foreach (var email in emails)
            {
                _emailSender.Send(email).Wait();
                base.Atualizar(email);
                Thread.Sleep(TimeSpan.FromMilliseconds(200));  // O SES da AWS tem o limite de 14 envios por segundo.
            }
        }
    }
}