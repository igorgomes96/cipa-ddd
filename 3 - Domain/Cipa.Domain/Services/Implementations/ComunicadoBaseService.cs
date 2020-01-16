using System;
using System.Collections.Generic;
using Cipa.Domain.Entities;
using Cipa.Domain.Services.Interfaces;

namespace Cipa.Domain.Services.Implementations
{
    public abstract class ComunicadoBaseService : IFormatadorEmailService
    {

        protected ComunicadoBaseService()
        {
            MapeamentoParametros = new Dictionary<string, Func<string>>();
            ParametrosUtilizados = new HashSet<string>();
        }

        public abstract ICollection<Email> FormatarEmails();

        protected Dictionary<string, Func<string>> MapeamentoParametros { get; }
        protected ICollection<string> ParametrosUtilizados { get; }


        protected virtual ICollection<Email> FormatarEmailPadrao(TemplateEmail templateEmail, string destinatarios)
        {
            var mensagem = SubstituirParametrosTemplate(templateEmail.Template);
            return new List<Email> {
                new Email(destinatarios, null, templateEmail.Assunto, mensagem)
            };
        }

        protected virtual string SubstituirParametrosTemplate(string templateEmail)
        {
            foreach (var parametro in ParametrosUtilizados)
                templateEmail = templateEmail.Replace(parametro, MapeamentoParametros[parametro]());

            return templateEmail;
        }


    }
}