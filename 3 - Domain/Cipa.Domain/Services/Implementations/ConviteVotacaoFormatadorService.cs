using System.Collections.Generic;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;

namespace Cipa.Domain.Services.Implementations
{
    public class ConviteVotacaoFormatadorService : ComunicadoEleicaoBaseService
    {
        public ConviteVotacaoFormatadorService(Eleicao eleicao) : base(eleicao)
        {
            ParametrosUtilizados.Add("@EMPRESA_CNPJ");
            ParametrosUtilizados.Add("@GESTAO");
            ParametrosUtilizados.Add("@ENDERECO");
            ParametrosUtilizados.Add("@CANDIDATOS");
            ParametrosUtilizados.Add("@PERIODO_VOTACAO");
            ParametrosUtilizados.Add("@TECNICO_SESMT");
            ParametrosUtilizados.Add("@TECNICO_CARGO");
        }

        public override ICollection<Email> FormatarEmails()
        {
            var templateEmail = BuscarTemplateEmail(ETipoTemplateEmail.ConviteParaVotacao);
           return FormatarEmailPadrao(templateEmail);
        }
    }
}