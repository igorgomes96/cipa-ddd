using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;
using Cipa.Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cipa.Infra.Data.Utils {
    public class EmailSender : IEmailSender
    {
        private IAmazonSimpleEmailService ses;
        private EmailConfiguration emailConfiguration;
        private ILogger<EmailSender> logger;
        public EmailSender(IAmazonSimpleEmailService ses, EmailConfiguration emailConfiguration, ILogger<EmailSender> logger) {
            this.ses = ses;
            this.emailConfiguration = emailConfiguration;
            this.logger = logger;
        }
        public async Task Send(Email email)
        {
            var sendRequest = new SendEmailRequest
                {
                    Source = emailConfiguration.Alias,
                    SourceArn = emailConfiguration.SESArn,
                    Destination = new Destination
                    {
                        ToAddresses = email.DestinatariosLista.ToList(),
                        CcAddresses = email.CopiasLista.ToList()
                    },
                    Message = new Message
                    {
                        Subject = new Content(email.Assunto),
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = "UTF-8",
                                Data = email.MensagemEstilizada
                            }
                        }
                    }
                };

                try
                {
                    logger.LogInformation("Enviando email pelo SES: {0}, para [{1}]", email.Assunto, email.Destinatarios);
                    var response = await ses.SendEmailAsync(sendRequest);
                    email.StatusEnvio = StatusEnvio.EnviadoComSucesso;
                    email.MensagemErro = null;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro ao enviar email.");
                    email.StatusEnvio = StatusEnvio.FalhaNoEnvio;
                    email.MensagemErro = ex.ToString();
                }
        }
    }
}