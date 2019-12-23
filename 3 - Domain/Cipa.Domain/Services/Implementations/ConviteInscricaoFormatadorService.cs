using System.Collections.Generic;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;

namespace Cipa.Domain.Services.Implementations
{
    public class ConviteInscricaoFormatadorService : ComunicadoEleicaoBaseService
    {
        public ConviteInscricaoFormatadorService(Eleicao eleicao) : base(eleicao)
        {
            ParametrosUtilizados.Add("@EMPRESA_CNPJ");
            ParametrosUtilizados.Add("@ENDERECO");
            ParametrosUtilizados.Add("@GESTAO");
            ParametrosUtilizados.Add("@PERIODO_INSCRICAO");
            ParametrosUtilizados.Add("@TECNICO_SESMT");
            ParametrosUtilizados.Add("@TECNICO_CARGO");
        }


        public override ICollection<Email> FormatarEmails()
        {
            var templateEmail = BuscarTemplateEmail(ETipoTemplateEmail.ConviteParaInscricao);
            return FormatarEmailPadrao(templateEmail);
        }
    }
}