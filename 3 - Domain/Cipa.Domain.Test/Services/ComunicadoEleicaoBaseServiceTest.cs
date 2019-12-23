using System;
using System.Text;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;
using Cipa.Domain.Services.Implementations;
using Cipa.Domain.Test.Mocks;
using Xunit;

namespace Cipa.Domain.Test.Services
{
    public class ComunicadoEleicaoBaseServiceTest
    {
        private readonly Grupo grupo;
        private readonly Empresa empresa;
        private readonly Estabelecimento estabelecimento;
        private readonly Usuario usuarioCriacao;
        private readonly Eleicao eleicao;
        private readonly ComunicadoEleicaoBaseService comunicadoEleicao;

        public ComunicadoEleicaoBaseServiceTest()
        {
            empresa = new Empresa("Soluções TI", "30271795000133") { Id = 1 };
            estabelecimento = new Estabelecimento("Uberlândia", "Av. Teste, 777, Bairro Santa Mônica", empresa.Id)
            {
                Empresa = empresa
            };
            var conta = new Conta { Id = 1 };
            conta.AdicionarEtapaPadrao(new EtapaPadraoConta("Convocação", "Convocação", 1, conta.Id, 1, ECodigoEtapaObrigatoria.Convocacao));
            conta.AdicionarEtapaPadrao(new EtapaPadraoConta("Inscrição", "Inscrição", 2, conta.Id, 2, ECodigoEtapaObrigatoria.Inscricao));
            conta.AdicionarEtapaPadrao(new EtapaPadraoConta("Votação", "Votação", 3, conta.Id, 1, ECodigoEtapaObrigatoria.Votacao));
            usuarioCriacao = new Usuario("gestor@email.com", "Gestor", "Técnico do SESMT")
            {
                Conta = conta
            };
            grupo = new Grupo("C-TESTE");
            grupo.Dimensionamentos.Add(new LinhaDimensionamento(5, 0, 1, 1));
            grupo.LimiteDimensionamento = new LimiteDimensionamento(5, 1, 1, 1);
            eleicao = new Eleicao(new DateTime(2020, 1, 1), 2, new DateTime(2020, 2, 28), usuarioCriacao, estabelecimento, grupo);
            eleicao.GerarCronograma();
            var templateEmailBuilder = new StringBuilder();
            templateEmailBuilder.Append("@ANO, @GESTAO, @DATA_COMPLETA, @EMPRESA_CNPJ, ");
            templateEmailBuilder.Append("@PERIODO_INSCRICAO, @PERIODO_VOTACAO, ");
            templateEmailBuilder.Append("@TECNICO_SESMT, @TECNICO_CARGO, @FIM_INSCRICAO, ");
            templateEmailBuilder.Append("@CANDIDATOS, @LINK");
            comunicadoEleicao = new ComunicadoEleicaoBaseServiceMock(eleicao,
                new TemplateEmail(ETipoTemplateEmail.EditalConvocacao, "Assunto Teste")
                {
                    Template = templateEmailBuilder.ToString()
                });
        }

        [Fact]
        public void FormatarEmailPadrao_TodosOsParametrosValidos_RetornaListaEmails()
        {
            var usuario1 = new Usuario("email1@email.com", "Usuário 1", "Cargo 1") { Id = 1 };
            var usuario2 = new Usuario("email2@email.com", "Usuário 2", "Cargo 2") { Id = 2, Senha = "A#SESR3" };
            var usuario3 = new Usuario("email3@email.com", "Usuário 3", "Cargo 3") { Id = 3 };
            var usuario4 = new Usuario("email4@email.com", "Usuário 4", "Cargo 4") { Id = 4, Senha = "A#SEsd98f" };
            var eleitor1 = new Eleitor("Eleitor 1", "email1@email.com") { Id = 1, Usuario = usuario1, Cargo = "Cargo 1" };
            var eleitor2 = new Eleitor("Eleitor 2", "email2@email.com") { Id = 2, Usuario = usuario2, Cargo = "Cargo 2" };
            var eleitor3 = new Eleitor("Eleitor 3", "email3@email.com") { Id = 3, Usuario = usuario3, Cargo = "Cargo 3" };
            var eleitor4 = new Eleitor("Eleitor 4", "email4@email.com") { Id = 4, Usuario = usuario4, Cargo = "Cargo 4" };

            eleicao.AdicionarEleitor(eleitor1);
            eleicao.AdicionarEleitor(eleitor2);
            eleicao.AdicionarEleitor(eleitor3);
            eleicao.AdicionarEleitor(eleitor4);

            eleicao.PassarParaProximaEtapa();
            eleicao.PassarParaProximaEtapa(); // Inscrições

            eleicao.FazerInscricao(eleitor1, "Objetivos 1");
            eleicao.FazerInscricao(eleitor3, "Objetivos 3");

            var emails = comunicadoEleicao.FormatarEmails();

            var mensagemSenhaCadastrada = new StringBuilder();
            mensagemSenhaCadastrada.Append("2020, 2020/2021, No 1º dia do mês de Janeiro de 2020, Soluções TI, ");
            mensagemSenhaCadastrada.Append("inscrita no CNPJ 30.271.795/0001-33, entre os dias 2 de Janeiro de 2020 ");
            mensagemSenhaCadastrada.Append("e 3 de Janeiro de 2020, no dia 4 de Janeiro de 2020, ");
            mensagemSenhaCadastrada.Append("Gestor, Técnico do SESMT, 3 de Janeiro de 2020, ");
            mensagemSenhaCadastrada.Append("<ol><li><strong>Eleitor 1</strong><br><small>Cargo 1</small></li>");
            mensagemSenhaCadastrada.Append("<li><strong>Eleitor 3</strong><br><small>Cargo 3</small></li></ol>, ");
            mensagemSenhaCadastrada.Append("http://localhost:4200/autenticacao/login");

            var mensagemSenhaNaoCadastrada1 = new StringBuilder();
            mensagemSenhaNaoCadastrada1.Append("2020, 2020/2021, No 1º dia do mês de Janeiro de 2020, Soluções TI, ");
            mensagemSenhaNaoCadastrada1.Append("inscrita no CNPJ 30.271.795/0001-33, entre os dias 2 de Janeiro de 2020 ");
            mensagemSenhaNaoCadastrada1.Append("e 3 de Janeiro de 2020, no dia 4 de Janeiro de 2020, ");
            mensagemSenhaNaoCadastrada1.Append("Gestor, Técnico do SESMT, 3 de Janeiro de 2020, ");
            mensagemSenhaNaoCadastrada1.Append("<ol><li><strong>Eleitor 1</strong><br><small>Cargo 1</small></li>");
            mensagemSenhaNaoCadastrada1.Append("<li><strong>Eleitor 3</strong><br><small>Cargo 3</small></li></ol>, ");
            mensagemSenhaNaoCadastrada1.Append($"http://localhost:4200/autenticacao/cadastro/{usuario1.CodigoRecuperacao.ToString()}");

            var mensagemSenhaNaoCadastrada2 = new StringBuilder();
            mensagemSenhaNaoCadastrada2.Append("2020, 2020/2021, No 1º dia do mês de Janeiro de 2020, Soluções TI, ");
            mensagemSenhaNaoCadastrada2.Append("inscrita no CNPJ 30.271.795/0001-33, entre os dias 2 de Janeiro de 2020 ");
            mensagemSenhaNaoCadastrada2.Append("e 3 de Janeiro de 2020, no dia 4 de Janeiro de 2020, ");
            mensagemSenhaNaoCadastrada2.Append("Gestor, Técnico do SESMT, 3 de Janeiro de 2020, ");
            mensagemSenhaNaoCadastrada2.Append("<ol><li><strong>Eleitor 1</strong><br><small>Cargo 1</small></li>");
            mensagemSenhaNaoCadastrada2.Append("<li><strong>Eleitor 3</strong><br><small>Cargo 3</small></li></ol>, ");
            mensagemSenhaNaoCadastrada2.Append($"http://localhost:4200/autenticacao/cadastro/{usuario3.CodigoRecuperacao.ToString()}");

            Assert.Collection(emails,
                (Email email) =>
                {
                    Assert.Equal(mensagemSenhaCadastrada.ToString(), email.Mensagem);
                    Assert.Collection(email.DestinatariosLista,
                        destinatario =>
                        {
                            Assert.Equal("email2@email.com", destinatario);
                        },
                        destinatario =>
                        {
                            Assert.Equal("email4@email.com", destinatario);
                        });
                },
                (Email email) =>
                {
                    Assert.Equal(mensagemSenhaNaoCadastrada1.ToString(), email.Mensagem);
                    Assert.Collection(email.DestinatariosLista,
                        destinatario =>
                        {
                            Assert.Equal("email1@email.com", destinatario);
                        });
                },
                (Email email) =>
                {
                    Assert.Equal(mensagemSenhaNaoCadastrada2.ToString(), email.Mensagem);
                    Assert.Collection(email.DestinatariosLista,
                        destinatario =>
                        {
                            Assert.Equal("email3@email.com", destinatario);
                        });
                }
            );


        }


    }
}