using System.Collections.Generic;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;

namespace Cipa.Domain.Services.Implementations
{
    public class EditalConvocacaoFormatadorService : ComunicadoEleicaoBaseService
    {
        public EditalConvocacaoFormatadorService(Eleicao eleicao) : base(eleicao)
        {
            ParametrosUtilizados.Add("@EMPRESA_CNPJ");
            ParametrosUtilizados.Add("@DATA_COMPLETA");
            ParametrosUtilizados.Add("@ENDERECO");
            ParametrosUtilizados.Add("@PERIODO_INSCRICAO");
            ParametrosUtilizados.Add("@PERIODO_VOTACAO");
            ParametrosUtilizados.Add("@TECNICO_SESMT");
            ParametrosUtilizados.Add("@TECNICO_CARGO");
        }

        public override ICollection<Email> FormatarEmails()
        {
            var templateEmail = BuscarTemplateEmail(ETipoTemplateEmail.EditalConvocacao);
            return FormatarEmailPadrao(templateEmail);
        }
    }
}