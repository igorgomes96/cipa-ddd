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
        public int MaxDestinatarios { get; set; } = 1;
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
        public StatusEnvio StatusEnvio { get; set; }
        public string MensagemErro { get; set; }
        public DateTime DataCadastro { get; private set; }

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
                    Body = MensagemEstilizada
                };

                if (!string.IsNullOrWhiteSpace(emailConfiguration.Alias) && emailConfiguration.UserName != emailConfiguration.Alias)
                {
                    mailMessage.From = new MailAddress(emailConfiguration.Alias);
                    mailMessage.Headers.Add("Sender", emailConfiguration.Alias);
                }
                DestinatariosLista.ToList().ForEach(to => mailMessage.To.Add(to));
                CopiasLista.ToList().ForEach(cc => mailMessage.CC.Add(cc));
                using (SmtpClient smtp = new SmtpClient(emailConfiguration.Host, emailConfiguration.Port))
                {
                    //IMPORTANT: Your smtp login email MUST be same as your FROM address. 
                    NetworkCredential credentials = new NetworkCredential(emailConfiguration.UserName, emailConfiguration.Password);
                    smtp.EnableSsl = true;
                    smtp.Credentials = credentials;
                    smtp.Send(mailMessage);
                }
                StatusEnvio = StatusEnvio.EnviadoComSucesso;
                MensagemErro = null;
            }
            catch (Exception e)
            {
                StatusEnvio = StatusEnvio.FalhaNoEnvio;
                MensagemErro = e.ToString();
            }
        }

        public IEnumerable<Email> DividirDestinatarios() {
            if (DestinatariosLista.Count() <= MaxDestinatarios) {
                return new [] { this };
            }

            string[] destinatarios = DestinatariosLista
                .Select((s, i) => new { Value = s, Index = i })
                .GroupBy(x => x.Index / MaxDestinatarios)
                .Select(grp => string.Join(",", grp.Select(g => g.Value)))
                .ToArray();
            
            return destinatarios.Select(d => new Email(d, Copias, Assunto, Mensagem));
        }

        public string MensagemEstilizada => @"
            <head>
                <style>
                    .template-email * {
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        margin: 0;
                    }

                    .template-email .container {
                        margin-top: 40px;
                        margin-bottom: 40px;
                        background: white;
                        max-width: 600px;
                        margin: 30px auto;
                        text-align: justify;
                        border: 1px solid #e9e9e9;
                        border-radius: 8px;
                    }

                    .template-email .titulo {
                        font-size: 24px;
                        text-align: center;
                        padding: 20px;
                        background: rgb(26, 179, 148);
                        color: white;
                        border-radius: 8px 8px 0 0;
                    }

                    .template-email .conteudo {
                        padding: 20px;
                    }

                    .template-email .assinatura {
                        margin-top: 30px;
                        text-align: center;
                    }
                </style>
            </head>
            <body class=""template-email"" style=""background-color: #f6f6f6;"">
                <div class=""container"">
                    <div class=""titulo"">"
                        + Assunto.Replace("[CIPA] ", "") +
            @"      </div>
                    <div class=""conteudo"">"
                        + Mensagem +
            @"      </div>
                </div>
            </body>";
    }
}
