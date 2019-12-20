using System.Collections.Generic;
using System.IO;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;

namespace Cipa.Domain.Services.Implementations
{
    public class ConviteInscricaoFormatadorService : ComunicadoEleicaoBaseService
    {
        public ConviteInscricaoFormatadorService(Eleicao eleicao) : base(eleicao)
        {
        }

        protected override ICollection<string> ParametrosUtilizados => 
            new HashSet<string> {
                "@EMPRESA_CNPJ", "@ENDERECO", "@GESTAO", "@PERIODO_INSCRICAO",
                "@TECNICO_SESMT", "@TECNICO_CARGO"
            };

        public override ICollection<Email> FormatarEmails()
        {
            return FormatarEmailPadrao(File.ReadAllText(ArquivosEmails.ConviteInscricao), AssuntosEmails.ConviteInscricao);
        }
    }
}