using System.Collections.Generic;
using System.Linq;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;

namespace Cipa.Domain.Services.Implementations
{
    public class ComunicadoSucessoMudancaEtapaService : ComunicadoEleicaoBaseService
    {
        public ComunicadoSucessoMudancaEtapaService(Eleicao eleicao) : base(eleicao)
        {
            MapeamentoParametros.Add("@ETAPA_ATUAL", () => eleicao.EtapaAtual?.Nome ?? "Finalização da Eleição");
            MapeamentoParametros.Add("@ETAPA_ANTERIOR", () => eleicao.EtapaAnterior?.Nome ?? "N/A - Início do Processo");
            ParametrosUtilizados.Add("@EMPRESA_CNPJ");
            ParametrosUtilizados.Add("@ETAPA_ATUAL");
            ParametrosUtilizados.Add("@ETAPA_ANTERIOR");
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
            var templateEmail = BuscarTemplateEmail(ETipoTemplateEmail.SucessoMudancaEtapaCronograma);
            return FormatarEmailPadrao(templateEmail);
        }
    }
}