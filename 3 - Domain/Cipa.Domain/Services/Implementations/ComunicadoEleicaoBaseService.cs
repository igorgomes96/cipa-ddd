using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;
using Cipa.Domain.Services.Interfaces;

namespace Cipa.Domain.Services.Implementations
{
    public abstract class ComunicadoEleicaoBaseService : IFormatadorEmailService
    {
        public ComunicadoEleicaoBaseService(Eleicao eleicao)
        {
            Eleicao = eleicao;
            MapeamentoParametros = new Dictionary<string, Func<string>> {
                { "@ANO", () => Eleicao.Gestao.ToString() },
                { "@GESTAO", ObterGestao },
                { "@ENDERECO", RetornarEnderecoEstabelecimento },
                { "@DATA_COMPLETA", () => RetornarDataCompleta(Eleicao.DataInicio) },
                { "@EMPRESA_CNPJ", RetornarEmpresaCNPJ },
                { "@PERIODO_INSCRICAO", () => RetornarPeriodo(CodigoEtapaObrigatoria.Inscricao) },
                { "@PERIODO_VOTACAO", () => RetornarPeriodo(CodigoEtapaObrigatoria.Votacao) },
                { "@TECNICO_SESMT", () => Eleicao.Usuario.Nome },
                { "@TECNICO_CARGO", () => eleicao.Usuario.Cargo ?? "Técnico em Segurança do Trabalho" },
                { "@FIM_INSCRICAO", () =>
                    {
                        var etapaInscricao = Eleicao.BuscarEtapaObrigatoria(CodigoEtapaObrigatoria.Inscricao);
                        return RetornarDataAbreviada(Eleicao.DataTerminoEtapa(etapaInscricao));
                    }
                },
                { "@CANDIDATOS", RetornarListaInscricoesHTML }
            };
        }

        protected Eleicao Eleicao { get; private set; }
        protected Dictionary<string, Func<string>> MapeamentoParametros { get; }
        protected abstract ICollection<string> ParametrosUtilizados { get; }
        protected IEnumerable<Usuario> UsuariosComSenhaCadastrada =>
            Eleicao.Eleitores.Where(e => e.Usuario.JaCadastrouSenha).Select(e => e.Usuario);
        protected IEnumerable<Usuario> UsuariosSemSenhaCadastrada =>
            Eleicao.Eleitores.Where(e => !e.Usuario.JaCadastrouSenha).Select(e => e.Usuario);
        public abstract ICollection<Email> FormatarEmails();

        protected ICollection<Email> FormatarEmailPadrao(string templateEmail, string assunto)
        {
            List<Email> emails = new List<Email>();
            var destinatarios = string.Join(",", UsuariosComSenhaCadastrada.Select(u => u.Email));
            var mensagemSemLink = SubstituirParametrosTemplate(templateEmail);
            var mensagem = mensagemSemLink.Replace("@LINK", LinkLogin);
            emails.Add(new Email(destinatarios, "", assunto, mensagem));

            foreach (var usuario in UsuariosSemSenhaCadastrada)
            {
                mensagem = mensagemSemLink.Replace("@LINK", LinkCadastro(usuario));
                emails.Add(new Email(usuario.Email, "", assunto, mensagem));
            }
            return emails;
        }

        protected string LinkLogin => $"{Links.URL}{Links.Login}";
        protected string LinkCadastro(Usuario usuario) =>
            $"{Links.URL}{Links.Cadastro}/{usuario.CodigoRecuperacao.ToString()}";

        protected virtual string SubstituirParametrosTemplate(string templateEmail)
        {
            foreach (var parametro in ParametrosUtilizados)
                templateEmail = templateEmail.Replace(parametro, MapeamentoParametros[parametro]());

            return templateEmail;
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

        protected virtual string RetornarPeriodo(CodigoEtapaObrigatoria etapaObrigatoria)
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