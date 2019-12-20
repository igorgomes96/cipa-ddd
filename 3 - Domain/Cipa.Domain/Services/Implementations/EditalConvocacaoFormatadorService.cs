using System.Collections.Generic;
using System.Linq;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;

namespace Cipa.Domain.Services.Implementations
{
    public class EditalConvocacaoFormatadorService : ComunicadoEleicaoBaseService
    {
        public EditalConvocacaoFormatadorService(Eleicao eleicao) : base(eleicao)
        {
        }

        protected override ICollection<string> ParametrosUtilizados =>
            new HashSet<string> {
                "@DATA_COMPLETA", "@EMPRESA_CNPJ", "@ENDERECO", "@PERIODO_INSCRICAO",
                "@PERIODO_VOTACAO", "@TECNICO_SESMT", "@TECNICO_CARGO"
            };

        public override ICollection<Email> FormatarEmails()
        {
            return FormatarEmailPadrao(ArquivosEmails.EditalConvocacao, AssuntosEmails.EditalConvocacao);
        }
    }
}