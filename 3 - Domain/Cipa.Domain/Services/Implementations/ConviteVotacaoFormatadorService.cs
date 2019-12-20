using System.Collections.Generic;
using System.Linq;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;

namespace Cipa.Domain.Services.Implementations
{
    public class ConviteVotacaoFormatadorService : ComunicadoEleicaoBaseService
    {
        public ConviteVotacaoFormatadorService(Eleicao eleicao) : base(eleicao)
        {
        }

        protected override ICollection<string> ParametrosUtilizados =>
            new HashSet<string> {
                "@EMPRESA_CNPJ", "@GESTAO", "@ENDERECO", "@CANDIDATOS",
                "@PERIODO_VOTACAO", "@TECNICO_SESMT", "@TECNICO_CARGO"
            };
        public override ICollection<Email> FormatarEmails()
        {
           return FormatarEmailPadrao(ArquivosEmails.ConviteVotacao, AssuntosEmails.ConviteVotacao);
        }
    }
}