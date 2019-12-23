using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Cipa.Domain.Helpers;

namespace Cipa.Domain.Entities
{
    public enum StatusEnvio
    {
        Pendente,
        Enviando,
        EnviadoComSucesso,
        FalhaNoEnvio
    }
    public class Email
    {
        public Email(string destinatarios, string copias, string assunto, string mensagem)
        {
            Destinatarios = destinatarios;
            Copias = copias;
            Assunto = assunto;
            Mensagem = mensagem;
            StatusEnvio = StatusEnvio.Pendente;
        }

        public long Id { get; set; }
        public string Destinatarios { get; set; }
        public string Copias { get; set; }
        public string Assunto { get; set; }
        public string Mensagem { get; set; }
        public StatusEnvio StatusEnvio { get; private set; }
        public string MensagemErro { get; private set; }

        public IEnumerable<string> DestinatariosLista { get => EmailsIndividuais(Destinatarios); }
        public IEnumerable<string> CopiasLista { get => EmailsIndividuais(Copias); }

        public void IniciarProcessoEnvio()
        {
            StatusEnvio = StatusEnvio.Enviando;
        }

        private IEnumerable<string> EmailsIndividuais(string addressesString)
        {
            List<string> enderecos = new List<string>();
            if (string.IsNullOrWhiteSpace(addressesString)) return enderecos;
            foreach (var mail in addressesString.Split(',').Where(add => !string.IsNullOrWhiteSpace(add)))
                enderecos.Add(mail);
            return enderecos;
        }

        public void Enviar(EmailConfiguration emailConfiguration)
        {
            try
            {
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(emailConfiguration.UserName),
                    Subject = Assunto,
                    IsBodyHtml = true,
                    Body = Mensagem
                };
                DestinatariosLista.ToList().ForEach(to => mailMessage.To.Add(to));
                CopiasLista.ToList().ForEach(cc => mailMessage.CC.Add(cc));
                using (SmtpClient smtp = new SmtpClient(emailConfiguration.Host, emailConfiguration.Port))
                {
                    //IMPORTANT: Your smtp login email MUST be same as your FROM address. 
                    NetworkCredential credentials = new NetworkCredential(emailConfiguration.UserName, emailConfiguration.Password);
                    smtp.Credentials = credentials;
                    smtp.Send(mailMessage);
                }
                StatusEnvio = StatusEnvio.EnviadoComSucesso;
                MensagemErro = null;
            }
            catch (Exception e)
            {
                StatusEnvio = StatusEnvio.FalhaNoEnvio;
                MensagemErro = e.Message;
            }
        }

    }
}
