using System.Collections.Generic;
using System.Linq;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;

namespace Cipa.Domain.Services.Implementations
{
    public class ComunicadoErroMudancaEtapaService : ComunicadoEleicaoBaseService
    {
        public ComunicadoErroMudancaEtapaService(Eleicao eleicao) : base(eleicao)
        {
            MapeamentoParametros.Add("@ERRO", () => Eleicao.EtapaAtual?.ErroMudancaEtapa);
            MapeamentoParametros.Add("@ETAPA_ATUAL", () => eleicao.EtapaAtual?.Nome ?? "N/A - Início do Processo");
            MapeamentoParametros.Add("@ETAPA_POSTERIOR", () => eleicao.EtapaPosterior?.Nome ?? "Finalização da Eleição");
            ParametrosUtilizados.Add("@EMPRESA_CNPJ");
            ParametrosUtilizados.Add("@ERRO");
            ParametrosUtilizados.Add("@ETAPA_ATUAL");
            ParametrosUtilizados.Add("@ETAPA_POSTERIOR");
        }

        protected override ICollection<Email> FormatarEmailPadrao(TemplateEmail templateEmail)
        {
            var usuariosSESMST = Eleicao.Conta.Usuarios
                .Select(x => x.Email).Aggregate((i, j) => $"{i},{j}");
            var mensagem = SubstituirParametrosTemplate(templateEmail.Template);
            return new List<Email> {
                new Email(usuariosSESMST, null, templateEmail.Assunto, mensagem)
            };
        }

        public override ICollection<Email> FormatarEmails()
        {
            var templateEmail = BuscarTemplateEmail(ETipoTemplateEmail.ErroMudancaEtapaCronograma);
            return FormatarEmailPadrao(templateEmail);
        }
    }
}