using System.Collections.Generic;
using System.Linq;
using Cipa.Domain.Entities;
using Cipa.Domain.Services.Implementations;

namespace Cipa.Domain.Test.Mocks
{
    public class ComunicadoEleicaoBaseServiceMock : ComunicadoEleicaoBaseService
    {
        public ComunicadoEleicaoBaseServiceMock(Eleicao eleicao, TemplateEmail templateEmail) : base(eleicao)
        {
            TemplateEmail = templateEmail;
            MapeamentoParametros.Keys.ToList().ForEach(p => ParametrosUtilizados.Add(p));
        }

        public TemplateEmail TemplateEmail { get; set; }

        public override ICollection<Email> FormatarEmails()
        {
            return FormatarEmailPadrao(TemplateEmail);
        }
    }
}