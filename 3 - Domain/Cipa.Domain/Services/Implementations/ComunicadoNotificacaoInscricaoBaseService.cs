using System.Collections;
using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Domain.Services.Implementations
{
    public abstract class ComunicadoNotificacaoInscricaoBaseService : ComunicadoEleicaoBaseService
    {

        public ComunicadoNotificacaoInscricaoBaseService(Inscricao inscricao) : base(inscricao.Eleicao)
        {
            Inscricao = inscricao;
            MapeamentoParametros.Add("@CANDIDATO_NOME", () => Inscricao.Eleitor.Nome);
            MapeamentoParametros.Add("@CANDIDATO_DADOS", RetornarDadosInscricao);
            ParametrosUtilizados.Add("@CANDIDATO_NOME");
            ParametrosUtilizados.Add("@CANDIDATO_DADOS");
            ParametrosUtilizados.Add("@TECNICO_SESMT");
            ParametrosUtilizados.Add("@TECNICO_CARGO");
        }

        private string RetornarDadosInscricao()
        {
            return $@"
                <ul>
                    <li>
                        <strong>Nome: </strong>{Inscricao.Eleitor.Nome}
                    </li>
                    <li>
                        <strong>Matrícula: </strong>{Inscricao.Eleitor.Matricula ?? ""} 
                    </li>
                    <li>
                        <strong>Admissão: </strong>{(Inscricao.Eleitor.DataAdmissao.HasValue ? RetornarDataAbreviada(Inscricao.Eleitor.DataAdmissao.Value) : "N/D")}
                    </li>
                    <li>
                        <strong>Nascimento: </strong>{(Inscricao.Eleitor.DataNascimento.HasValue ? RetornarDataAbreviada(Inscricao.Eleitor.DataNascimento.Value) : "N/D")}
                    </li>
                    <li>
                        <strong>Área: </strong>{Inscricao.Eleitor.Area ?? ""}
                    </li>
                    <li>
                        <strong>Cargo: </strong>{Inscricao.Eleitor.Cargo ?? ""}
                    </li>
                    <li>
                        <strong>Objetivos: </strong>{Inscricao.Objetivos ?? ""}
                    </li>
                </ul>
            ";
        }

        protected Inscricao Inscricao { get; }

        protected override ICollection<Email> FormatarEmailPadrao(TemplateEmail templateEmail, string destinatarios)
        {
            var emails = base.FormatarEmailPadrao(templateEmail, destinatarios);
            var emailGestor = Inscricao.Eleitor.EmailGestor;
            if (!string.IsNullOrWhiteSpace(emailGestor))
            {
                foreach (var email in emails)
                {
                    if (string.IsNullOrWhiteSpace(email.Copias))
                        email.Copias = emailGestor;
                    else
                        email.Copias += $",{emailGestor}";
                }
            }
            return emails;
        }

    }
}