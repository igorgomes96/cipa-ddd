using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;
using Cipa.Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cipa.Infra.Data.Utils
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration emailConfiguration;
        private readonly ILogger<EmailSender> logger;
        public EmailSender(EmailConfiguration emailConfiguration, ILogger<EmailSender> logger)
        {
            this.emailConfiguration = emailConfiguration;
            this.logger = logger;
        }

        public async Task Send(Email email)
        {
            // Replace sender@example.com with your "From" address. 
            // This address must be verified with Amazon SES.
            string FROM = emailConfiguration.Alias;
            string FROMNAME = emailConfiguration.Name;

            // Replace smtp_username with your Amazon SES SMTP user name.
            string SMTP_USERNAME = emailConfiguration.UserName;

            // Replace smtp_password with your Amazon SES SMTP password.
            string SMTP_PASSWORD = emailConfiguration.Password;

            // If you're using Amazon SES in a region other than US West (Oregon), 
            // replace email-smtp.us-west-2.amazonaws.com with the Amazon SES SMTP  
            // endpoint in the appropriate AWS Region.
            string HOST = "email-smtp.us-east-2.amazonaws.com";

            // The port you will connect to on the Amazon SES SMTP endpoint. We
            // are choosing port 587 because we will use STARTTLS to encrypt
            // the connection.
            int PORT = 587;

            // The subject line of the email
            string SUBJECT = email.Assunto;

            // The body of the email
            string BODY = email.Mensagem;

            // Create and build a new MailMessage object
            MailMessage message = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress(FROM, FROMNAME),
                Subject = SUBJECT,
                Body = BODY
            };
            foreach (var to in email.DestinatariosLista)
                message.To.Add(new MailAddress(to));

            using var client = new SmtpClient(HOST, PORT);
            // Pass SMTP credentials
            client.Credentials = new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

            // Enable SSL encryption
            client.EnableSsl = true;

            // Try to send the message. Show status in console.
            try
            {
                Console.WriteLine("Attempting to send email...");
                await client.SendMailAsync(message);
                email.StatusEnvio = StatusEnvio.EnviadoComSucesso;
                email.MensagemErro = null;
                Console.WriteLine("Email sent!");
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