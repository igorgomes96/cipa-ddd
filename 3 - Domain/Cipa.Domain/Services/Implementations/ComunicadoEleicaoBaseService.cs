using System;
using System.Collections.Generic;
using System.Linq;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;

namespace Cipa.Domain.Services.Implementations
{
    public abstract class ComunicadoEleicaoBaseService : ComunicadoBaseService
    {
        public ComunicadoEleicaoBaseService(Eleicao eleicao)
        {
            Eleicao = eleicao;
            MapeamentoParametros.Add("@ANO", () => Eleicao.Gestao.ToString());
            MapeamentoParametros.Add("@GESTAO", ObterGestao);
            MapeamentoParametros.Add("@ENDERECO", RetornarEnderecoEstabelecimento);
            MapeamentoParametros.Add("@DATA_COMPLETA", () => RetornarDataCompleta(Eleicao.DataInicio));
            MapeamentoParametros.Add("@EMPRESA_CNPJ", RetornarEmpresaCNPJ);
            MapeamentoParametros.Add("@PERIODO_INSCRICAO", () => RetornarPeriodo(ECodigoEtapaObrigatoria.Inscricao));
            MapeamentoParametros.Add("@PERIODO_VOTACAO", () => RetornarPeriodo(ECodigoEtapaObrigatoria.Votacao));
            MapeamentoParametros.Add("@TECNICO_SESMT", () => Eleicao.Usuario.Nome);
            MapeamentoParametros.Add("@TECNICO_CARGO", () => Eleicao.Usuario.Cargo);
            MapeamentoParametros.Add("@FIM_INSCRICAO", RetornarDataFimInscricao);
            MapeamentoParametros.Add("@CANDIDATOS", RetornarListaInscricoesHTML);
        }

        protected Eleicao Eleicao { get; private set; }

        protected IEnumerable<Usuario> UsuariosComSenhaCadastrada =>
            Eleicao.Eleitores.Where(e => e.Usuario.JaCadastrouSenha).Select(e => e.Usuario);
        protected IEnumerable<Usuario> UsuariosSemSenhaCadastrada =>
            Eleicao.Eleitores.Where(e => !e.Usuario.JaCadastrouSenha).Select(e => e.Usuario);

        protected string RetornarDataFimInscricao()
        {
            var etapaInscricao = Eleicao.BuscarEtapaObrigatoria(ECodigoEtapaObrigatoria.Inscricao);
            return RetornarDataAbreviada(Eleicao.DataTerminoEtapa(etapaInscricao));
        }

        protected virtual ICollection<Email> FormatarEmailPadrao(TemplateEmail templateEmail)
        {
            List<Email> emails = new List<Email>();
            var mensagemSemLink = SubstituirParametrosTemplate(templateEmail.Template);
            if (UsuariosComSenhaCadastrada.Any())
            {
                var destinatarios = string.Join(",", UsuariosComSenhaCadastrada.Select(u => u.Email));
                var mensagem = mensagemSemLink.Replace("@LINK", LinkLogin);
                emails.Add(new Email(destinatarios, "", templateEmail.Assunto, mensagem));
            }

            foreach (var usuario in UsuariosSemSenhaCadastrada)
            {
                var mensagem = mensagemSemLink.Replace("@LINK", LinkCadastro(usuario));
                emails.Add(new Email(usuario.Email, "", templateEmail.Assunto, mensagem));
            }

            return emails;
        }


        protected string LinkLogin => $"{Links.URL}{Links.Login}";
        protected string LinkCadastro(Usuario usuario) =>
            $"{Links.URL}{Links.Cadastro}/{usuario.CodigoRecuperacao.ToString()}";

        protected virtual TemplateEmail BuscarTemplateEmail(ETipoTemplateEmail tipoTemplateEmail)
        {
            var template = Eleicao.Conta.BuscarTemplateEmail(tipoTemplateEmail);
            if (template == null)
                throw new CustomException("Template de e-mail não localizado.");
            return template;
        }

        protected virtual string RetornarDataAbreviada(DateTime data) =>
            $"{data.Day} de {Util.NomeMes(data.Month)} de {data.Year}";

        protected virtual string ObterGestao()
        {
            if (Eleicao.DuracaoGestao > 1)
                return $"{Eleicao.Gestao}/{Eleicao.Gestao + Eleicao.DuracaoGestao - 1}";
            return Eleicao.Gestao.ToString();
        }


        protected virtual string RetornarDataCompleta(DateTime data)
        {
            var dia = data.Day;
            var mes = data.Month;
            var ano = data.Year;

            string dataStr = string.Empty;
            if (dia == 1)
                dataStr = "No 1º dia ";
            else
                dataStr = $"Aos {dia} dias ";

            dataStr += $"do mês de {Util.NomeMes(mes)} de {ano}";
            return dataStr;
        }

        protected string ObterHorario(DateTime? data) => data.HasValue ? data.Value.ToString("dd/MM/yyyy HH:mm") : "";

        protected virtual string RetornarPeriodo(ECodigoEtapaObrigatoria etapaObrigatoria)
        {
            var etapa = Eleicao.BuscarEtapaObrigatoria(etapaObrigatoria);
            return RetornarPeriodo(etapa.DataPrevista, Eleicao.DataTerminoEtapa(etapa));
        }

        protected virtual string RetornarPeriodo(DateTime dataInicio, DateTime? dataFim = null)
        {
            if (dataFim == null || dataInicio == dataFim)
                return $"no dia {RetornarDataAbreviada(dataInicio)}";
            return $"entre os dias {RetornarDataAbreviada(dataInicio)} e {RetornarDataAbreviada(dataFim.Value)}";
        }

        protected virtual string RetornarEnderecoEstabelecimento() =>
            $"{Eleicao.Estabelecimento.Endereco}, na cidade de {Eleicao.Estabelecimento.Cidade}";

        protected virtual string RetornarEmpresaCNPJ()
        {
            var empresa = $"{Eleicao.Estabelecimento.Empresa.RazaoSocial}";
            if (!string.IsNullOrWhiteSpace(Eleicao.Estabelecimento.Empresa.Cnpj))
                empresa += $", inscrita no CNPJ {Eleicao.Estabelecimento.Empresa.CnpjFormatado}";
            return empresa;
        }

        protected virtual string RetornarListaInscricoesHTML() =>
            Eleicao.Inscricoes.Aggregate("<ol>",
                (acc, cur) => $"{acc}<li><strong>{cur.Eleitor.Nome}</strong><br><small>{cur.Eleitor.Cargo}</small></li>"
            ) + "</ol>";
    }
}