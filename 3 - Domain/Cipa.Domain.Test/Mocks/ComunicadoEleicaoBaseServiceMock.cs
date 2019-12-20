using System.Collections.Generic;
using System.Linq;
using Cipa.Domain.Entities;
using Cipa.Domain.Services.Implementations;

namespace Cipa.Domain.Test.Mocks
{
    public class ComunicadoEleicaoBaseServiceMock : ComunicadoEleicaoBaseService
    {
        public ComunicadoEleicaoBaseServiceMock(Eleicao eleicao, string templateEmail, string assuntoEmail) : base(eleicao)
        {
            TemplateEmail = templateEmail;
            AssuntoEmail = assuntoEmail;
        }

        protected override ICollection<string> ParametrosUtilizados => MapeamentoParametros.Keys.ToList();
        public string TemplateEmail { get; set; }
        public string AssuntoEmail { get; set; }

        public override ICollection<Email> FormatarEmails()
        {
            return FormatarEmailPadrao(TemplateEmail, AssuntoEmail);
        }
    }
}