using System.Collections.Generic;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;

namespace Cipa.Domain.Services.Implementations
{
    public class NotificacaoInscricaoReprovadaService : ComunicadoNotificacaoInscricaoBaseService
    {
        public NotificacaoInscricaoReprovadaService(Inscricao inscricao) : base(inscricao)
        {
            MapeamentoParametros.Add("@REPROVACAO_DADOS", RetornarDadosReprovacao);
            ParametrosUtilizados.Add("@REPROVACAO_DADOS");
        }

        public override ICollection<Email> FormatarEmails()
        {
            var templateEmail = Inscricao.Eleicao.Conta.BuscarTemplateEmail(ETipoTemplateEmail.InscricaoReprovada);
            return FormatarEmailPadrao(templateEmail, Inscricao.Eleitor.Email);
        }

        private string RetornarDadosReprovacao()
        {
            var reprovacao = Inscricao.BuscarUltimaReprovacao();
            return $@"
                <ul>
                    <li>
                        <strong>Horário: </strong>{ObterHorario(reprovacao.DataCadastro)}
                    </li>
                    <li>
                        <strong>Aprovador: </strong>{reprovacao.NomeAprovador ?? ""}
                    </li>
                    <li>
                        <strong>Email do Aprovador: </strong>{reprovacao.EmailAprovador ?? ""}
                    </li>
                    <li>
                        <strong>Motivo da Reprovação: </strong>{reprovacao.MotivoReprovacao ?? ""}
                    </li>
                </ul>
                ";
        }
    }
}