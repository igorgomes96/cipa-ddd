using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cipa.WebApi.Migrations
{
    public partial class Inicio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EtapasObrigatorias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Nome = table.Column<string>(maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(maxLength: 4000, nullable: true),
                    Ordem = table.Column<int>(nullable: false),
                    DuracaoMinima = table.Column<int>(nullable: true),
                    PrazoMandatoAnterior = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtapasObrigatorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grupos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CodigoGrupo = table.Column<string>(maxLength: 10, nullable: false),
                    Limite = table.Column<int>(nullable: false),
                    IntervaloAcrescimo = table.Column<int>(nullable: false),
                    AcrescimoEfetivos = table.Column<int>(nullable: false),
                    AcrescimoSuplentes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plano",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(nullable: true),
                    ValorMes = table.Column<decimal>(nullable: true),
                    ValorAno = table.Column<decimal>(nullable: true),
                    QtdaEstabelecimentos = table.Column<int>(nullable: false),
                    DataInicio = table.Column<DateTime>(nullable: false),
                    DataFim = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plano", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LinhasDimensionamentos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Minimo = table.Column<int>(nullable: false),
                    Maximo = table.Column<int>(nullable: false),
                    QtdaEfetivos = table.Column<int>(nullable: false),
                    QtdaSuplentes = table.Column<int>(nullable: false),
                    GrupoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinhasDimensionamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinhasDimensionamentos_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlanoId = table.Column<int>(nullable: true),
                    Ativa = table.Column<bool>(nullable: false),
                    QtdaEstabelecimentos = table.Column<int>(nullable: false),
                    DataInicio = table.Column<DateTime>(nullable: false),
                    DataFim = table.Column<DateTime>(nullable: false),
                    DataCadastro = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contas_Plano_PlanoId",
                        column: x => x.PlanoId,
                        principalTable: "Plano",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RazaoSocial = table.Column<string>(maxLength: 255, nullable: false),
                    Cnpj = table.Column<string>(maxLength: 14, nullable: true),
                    InformacoesGerais = table.Column<string>(maxLength: 255, nullable: true),
                    ContaId = table.Column<int>(nullable: false),
                    Ativa = table.Column<bool>(nullable: false),
                    DataCadastro = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Empresas_Contas_ContaId",
                        column: x => x.ContaId,
                        principalTable: "Contas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EtapasPadroesContas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(maxLength: 4000, nullable: true),
                    Ordem = table.Column<int>(nullable: false),
                    ContaId = table.Column<int>(nullable: false),
                    EtapaObrigatoriaId = table.Column<int>(nullable: true),
                    DuracaoPadrao = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtapasPadroesContas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EtapasPadroesContas_Contas_ContaId",
                        column: x => x.ContaId,
                        principalTable: "Contas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EtapasPadroesContas_EtapasObrigatorias_EtapaObrigatoriaId",
                        column: x => x.EtapaObrigatoriaId,
                        principalTable: "EtapasObrigatorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    Nome = table.Column<string>(nullable: true),
                    Senha = table.Column<string>(nullable: true),
                    ContaId = table.Column<int>(nullable: true),
                    Perfil = table.Column<string>(nullable: true),
                    CodigoRecuperacao = table.Column<Guid>(nullable: true),
                    ExpiracaoCodigoRecuperacao = table.Column<DateTime>(nullable: true),
                    Cargo = table.Column<string>(nullable: true),
                    DataCadastro = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Contas_ContaId",
                        column: x => x.ContaId,
                        principalTable: "Contas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Estabelecimentos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmpresaId = table.Column<int>(nullable: false),
                    Cidade = table.Column<string>(maxLength: 100, nullable: false),
                    Endereco = table.Column<string>(maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(maxLength: 255, nullable: true),
                    GrupoId = table.Column<int>(nullable: true),
                    Ativo = table.Column<bool>(nullable: false),
                    DataCadastro = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estabelecimentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estabelecimentos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Estabelecimentos_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Eleicoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Gestao = table.Column<int>(nullable: false),
                    DuracaoGestao = table.Column<int>(nullable: false),
                    EstabelecimentoId = table.Column<int>(nullable: false),
                    DataInicio = table.Column<DateTime>(nullable: false),
                    UsuarioCriacaoId = table.Column<int>(nullable: false),
                    ContaId = table.Column<int>(nullable: false),
                    DataCadastro = table.Column<DateTime>(nullable: false),
                    DataFinalizacao = table.Column<DateTime>(nullable: true),
                    GrupoId = table.Column<int>(nullable: false),
                    TerminoMandatoAnterior = table.Column<DateTime>(nullable: true),
                    DimensionamentoMinEleitores = table.Column<int>(nullable: false),
                    DimensionamentoMaxEleitores = table.Column<int>(nullable: false),
                    DimensionamentoQtdaEfetivos = table.Column<int>(nullable: false),
                    DimensionamentoQtdaSuplentes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eleicoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Eleicoes_Contas_ContaId",
                        column: x => x.ContaId,
                        principalTable: "Contas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Eleicoes_Estabelecimentos_EstabelecimentoId",
                        column: x => x.EstabelecimentoId,
                        principalTable: "Estabelecimentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Eleicoes_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Eleicoes_Usuarios_UsuarioCriacaoId",
                        column: x => x.UsuarioCriacaoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Eleitores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(maxLength: 255, nullable: false),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    Matricula = table.Column<string>(maxLength: 50, nullable: true),
                    Area = table.Column<string>(maxLength: 255, nullable: true),
                    Cargo = table.Column<string>(maxLength: 255, nullable: true),
                    DataNascimento = table.Column<DateTime>(nullable: true),
                    DataAdmissao = table.Column<DateTime>(nullable: true),
                    EleicaoId = table.Column<int>(nullable: false),
                    UsuarioId = table.Column<int>(nullable: false),
                    NomeGestor = table.Column<string>(maxLength: 255, nullable: true),
                    EmailGestor = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eleitores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Eleitores_Eleicoes_EleicaoId",
                        column: x => x.EleicaoId,
                        principalTable: "Eleicoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Eleitores_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EtapasCronogramas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(maxLength: 4000, nullable: true),
                    Ordem = table.Column<int>(nullable: false),
                    EleicaoId = table.Column<int>(nullable: false),
                    DataPrevista = table.Column<DateTime>(nullable: false),
                    DataRealizada = table.Column<DateTime>(nullable: true),
                    EtapaObrigatoriaId = table.Column<int>(nullable: true),
                    PosicaoEtapa = table.Column<int>(nullable: false),
                    ErroMudancaEtapa = table.Column<string>(maxLength: 10000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtapasCronogramas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EtapasCronogramas_Eleicoes_EleicaoId",
                        column: x => x.EleicaoId,
                        principalTable: "Eleicoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EtapasCronogramas_EtapasObrigatorias_EtapaObrigatoriaId",
                        column: x => x.EtapaObrigatoriaId,
                        principalTable: "EtapasObrigatorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inscricoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Votos = table.Column<int>(nullable: false),
                    StatusInscricao = table.Column<int>(nullable: false),
                    EleitorId = table.Column<int>(nullable: false),
                    EleicaoId = table.Column<int>(nullable: false),
                    Foto = table.Column<string>(maxLength: 255, nullable: true),
                    Objetivos = table.Column<string>(maxLength: 255, nullable: true),
                    EmailAprovador = table.Column<string>(maxLength: 100, nullable: true),
                    NomeAprovador = table.Column<string>(maxLength: 255, nullable: true),
                    HorarioAprovacao = table.Column<DateTime>(nullable: true),
                    DataCadastro = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscricoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inscricoes_Eleicoes_EleicaoId",
                        column: x => x.EleicaoId,
                        principalTable: "Eleicoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inscricoes_Eleitores_EleitorId",
                        column: x => x.EleitorId,
                        principalTable: "Eleitores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EleitorId = table.Column<int>(nullable: false),
                    EleicaoId = table.Column<int>(nullable: false),
                    IP = table.Column<string>(maxLength: 15, nullable: false),
                    DataCadastro = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votos_Eleicoes_EleicaoId",
                        column: x => x.EleicaoId,
                        principalTable: "Eleicoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votos_Eleitores_EleitorId",
                        column: x => x.EleitorId,
                        principalTable: "Eleitores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reprovacoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    InscricaoId = table.Column<int>(nullable: false),
                    MotivoReprovacao = table.Column<string>(maxLength: 255, nullable: false),
                    DataCadastro = table.Column<DateTime>(nullable: false),
                    EmailAprovador = table.Column<string>(maxLength: 255, nullable: true),
                    NomeAprovador = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reprovacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reprovacoes_Inscricoes_InscricaoId",
                        column: x => x.InscricaoId,
                        principalTable: "Inscricoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Contas",
                columns: new[] { "Id", "Ativa", "DataCadastro", "DataFim", "DataInicio", "PlanoId", "QtdaEstabelecimentos" },
                values: new object[] { 1, true, new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2 });

            migrationBuilder.InsertData(
                table: "EtapasObrigatorias",
                columns: new[] { "Id", "Descricao", "DuracaoMinima", "Nome", "Ordem", "PrazoMandatoAnterior" },
                values: new object[,]
                {
                    { 1, "No início do processo eleitoral, o empregador deve convocar as eleições para a escolha dos representantes dos empregados na CIPA. Essa convocação precisa ocorrer no prazo mínimo de 60 dias antes do término do mandato em curso (NR-05 - 5.38)", null, "Convocação para a Eleição", 1, 60 },
                    { 2, "A Norma Regulamentadora 05 também determina que, no prazo mínimo de 55 dias antes do término do mandato em curso, seja constituída a Comissão Eleitoral (CE). Essa comissão deve ser formada pelos membros atuais da CIPA e será responsável pela organização e acompanhamento do processo eleitoral (NR-05 5.39). O edital de convocação também deve ser enviado para o sindicatos 5 dias após sua publicação.", null, "Formação da Comissão Eleitoral", 2, 55 },
                    { 3, "Nessa etapa, é preciso publicar e divulgar o edital de inscrição para a CIPA, em locais de fácil acesso e visualização. Atente-se ao prazo! A NR-5 determina que a publicação ocorra no prazo mínimo de 45 dias antes do término do mandato em curso. (NR-05 - 5.40 a)", null, "Edital de Inscrição dos Candidatos", 3, 45 },
                    { 4, "Nessa etapa, o sistema libera acesso aos eleitores cadastrados nessa eleição para eles realizarem sua inscrição. Essa etapa deve ter duração mínima de 15 dias. É importante lembrar que qualquer empregado do estabelecimento, independentemente do setor ou local de trabalho, pode se inscrever. (NR-05 5.40 b. c.)", 15, "Inscrição dos Candidatos", 4, 0 },
                    { 5, "Nessa etapa, todos os eleitores podem acessar o sistema e escolher um dos candidatos como representante para o próximo mandato da CIPA. A eleição deve ser realizada em um dia normal de trabalho e 30 dias antes do término do mandato em curso, no mínimo. (NR-05 5.40 e. f.)", null, "Votação", 5, 30 },
                    { 6, "Os candidatos votados e não eleitos devem estar relacionados nessa ata, em ordem decrescente de votos para nomeação posterior, em caso de vacância de suplentes. Não se preocupe, a criação desse documento é por nossa conta!", null, "Ata de Eleição", 6, 0 }
                });

            migrationBuilder.InsertData(
                table: "Grupos",
                columns: new[] { "Id", "CodigoGrupo", "AcrescimoEfetivos", "AcrescimoSuplentes", "IntervaloAcrescimo", "Limite" },
                values: new object[,]
                {
                    { 25, "C-19", 1, 1, 2500, 10000 },
                    { 26, "C-20", 2, 1, 2500, 10000 },
                    { 27, "C-21", 1, 1, 2500, 10000 },
                    { 28, "C-22", 2, 2, 2500, 10000 },
                    { 29, "C-23", 1, 1, 2500, 10000 },
                    { 30, "C-24", 2, 2, 2500, 10000 },
                    { 31, "C-24a", 1, 1, 2500, 10000 },
                    { 32, "C-24b", 2, 2, 2500, 10000 },
                    { 33, "C-24c", 1, 1, 2500, 10000 },
                    { 37, "C-27", 1, 1, 2500, 10000 },
                    { 35, "C-25", 1, 1, 2500, 10000 },
                    { 36, "C-26", 1, 1, 2500, 10000 },
                    { 24, "C-18a", 2, 2, 2500, 10000 },
                    { 38, "C-28", 1, 1, 2500, 10000 },
                    { 39, "C-29", 1, 1, 2500, 10000 },
                    { 40, "C-30", 2, 1, 2500, 10000 },
                    { 41, "C-31", 1, 1, 2500, 10000 },
                    { 42, "C-32", 1, 1, 2500, 10000 },
                    { 43, "C-33", 1, 1, 2500, 10000 },
                    { 34, "C-24d", 1, 1, 2500, 10000 },
                    { 23, "C-18", 2, 2, 2500, 10000 },
                    { 19, "C-14a", 1, 1, 2500, 10000 },
                    { 21, "C-16", 2, 2, 2500, 10000 },
                    { 1, "C-1", 2, 2, 2500, 10000 },
                    { 2, "C-1a", 2, 2, 2500, 10000 },
                    { 3, "C-2", 2, 1, 2500, 10000 },
                    { 4, "C-3", 2, 2, 2500, 10000 },
                    { 5, "C-3a", 1, 1, 2500, 10000 },
                    { 6, "C-4", 1, 1, 2500, 10000 },
                    { 7, "C-5", 2, 2, 2500, 10000 },
                    { 8, "C-5a", 1, 1, 2500, 10000 },
                    { 9, "C-6", 2, 2, 2500, 10000 },
                    { 22, "C-17", 2, 2, 2500, 10000 },
                    { 10, "C-7", 1, 1, 2500, 10000 },
                    { 12, "C-8", 1, 1, 2500, 10000 },
                    { 13, "C-9", 1, 1, 2500, 10000 },
                    { 14, "C-10", 2, 2, 2500, 10000 },
                    { 15, "C-11", 2, 2, 2500, 10000 },
                    { 16, "C-12", 2, 2, 2500, 10000 },
                    { 17, "C-13", 2, 2, 2500, 10000 },
                    { 18, "C-14", 2, 2, 2500, 10000 },
                    { 44, "C-34", 2, 2, 2500, 10000 },
                    { 20, "C-15", 2, 2, 2500, 10000 },
                    { 11, "C-7a", 2, 2, 2500, 10000 },
                    { 45, "C-35", 1, 1, 2500, 10000 }
                });

            migrationBuilder.InsertData(
                table: "Empresas",
                columns: new[] { "Id", "Ativa", "Cnpj", "ContaId", "DataCadastro", "InformacoesGerais", "RazaoSocial" },
                values: new object[] { 1, true, "01540533000390", 1, new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Empresa Teste" });

            migrationBuilder.InsertData(
                table: "EtapasPadroesContas",
                columns: new[] { "Id", "ContaId", "Descricao", "DuracaoPadrao", "EtapaObrigatoriaId", "Nome", "Ordem" },
                values: new object[,]
                {
                    { 1, 1, "No início do processo eleitoral, o empregador deve convocar as eleições para a escolha dos representantes dos empregados na CIPA. Essa convocação precisa ocorrer no prazo mínimo de 60 dias antes do término do mandato em curso (NR-05 - 5.38)", 5, 1, "Convocação para a Eleição", 1 },
                    { 2, 1, "A Norma Regulamentadora 05 também determina que, no prazo mínimo de 55 dias antes do término do mandato em curso, seja constituída a Comissão Eleitoral (CE). Essa comissão deve ser formada pelos membros atuais da CIPA e será responsável pela organização e acompanhamento do processo eleitoral (NR-05 5.39). O edital de convocação também deve ser enviado para o sindicatos 5 dias após sua publicação.", 5, 2, "Formação da Comissão Eleitoral", 2 },
                    { 3, 1, "Nessa etapa, é preciso publicar e divulgar o edital de inscrição para a CIPA, em locais de fácil acesso e visualização. Atente-se ao prazo! A NR-5 determina que a publicação ocorra no prazo mínimo de 45 dias antes do término do mandato em curso. (NR-05 - 5.40 a)", 5, 3, "Edital de Inscrição dos Candidatos", 3 },
                    { 4, 1, "Nessa etapa, o sistema libera acesso aos eleitores cadastrados nessa eleição para eles realizarem sua inscrição. Essa etapa deve ter duração mínima de 15 dias. É importante lembrar que qualquer empregado do estabelecimento, independentemente do setor ou local de trabalho, pode se inscrever. (NR-05 5.40 b. c.)", 5, 4, "Inscrição dos Candidatos", 4 },
                    { 5, 1, "Nessa etapa, todos os eleitores podem acessar o sistema e escolher um dos candidatos como representante para o próximo mandato da CIPA. A eleição deve ser realizada em um dia normal de trabalho e 30 dias antes do término do mandato em curso, no mínimo. (NR-05 5.40 e. f.)", 5, 5, "Votação", 5 },
                    { 6, 1, "Os candidatos votados e não eleitos devem estar relacionados nessa ata, em ordem decrescente de votos para nomeação posterior, em caso de vacância de suplentes. Não se preocupe, a criação desse documento é por nossa conta!", 5, 6, "Ata de Eleição", 6 }
                });

            migrationBuilder.InsertData(
                table: "LinhasDimensionamentos",
                columns: new[] { "Id", "GrupoId", "Maximo", "Minimo", "QtdaEfetivos", "QtdaSuplentes" },
                values: new object[,]
                {
                    { 249, 30, 50, 20, 1, 1 },
                    { 248, 30, 19, 0, 0, 0 },
                    { 247, 29, 10000, 5001, 6, 5 },
                    { 246, 29, 5000, 2501, 5, 4 },
                    { 245, 29, 2500, 1001, 4, 3 },
                    { 244, 29, 1000, 501, 3, 3 },
                    { 243, 29, 500, 101, 2, 2 },
                    { 242, 29, 100, 51, 1, 1 },
                    { 241, 29, 50, 0, 0, 0 },
                    { 237, 28, 1000, 501, 6, 5 },
                    { 239, 28, 5000, 2501, 10, 8 },
                    { 238, 28, 2500, 1001, 8, 6 },
                    { 250, 30, 100, 51, 2, 2 },
                    { 236, 28, 500, 141, 4, 3 },
                    { 235, 28, 140, 101, 3, 3 },
                    { 234, 28, 100, 51, 2, 2 },
                    { 233, 28, 50, 20, 1, 1 },
                    { 232, 28, 19, 0, 0, 0 },
                    { 231, 27, 10000, 5001, 6, 5 },
                    { 240, 28, 10000, 5001, 12, 9 },
                    { 251, 30, 140, 101, 4, 3 },
                    { 254, 30, 2500, 1001, 8, 7 },
                    { 253, 30, 1000, 501, 6, 5 },
                    { 272, 33, 50, 0, 0, 0 },
                    { 271, 32, 10000, 5001, 15, 12 },
                    { 270, 32, 5000, 2501, 12, 9 },
                    { 269, 32, 2500, 1001, 9, 7 },
                    { 268, 32, 1000, 501, 6, 4 },
                    { 267, 32, 500, 101, 4, 3 },
                    { 266, 32, 100, 51, 3, 3 },
                    { 265, 32, 50, 20, 1, 1 },
                    { 264, 32, 19, 0, 0, 0 },
                    { 263, 31, 10000, 5001, 6, 4 },
                    { 262, 31, 5000, 2501, 5, 4 },
                    { 261, 31, 2500, 1001, 4, 3 },
                    { 260, 31, 1000, 501, 3, 3 },
                    { 259, 31, 500, 101, 2, 2 },
                    { 258, 31, 100, 51, 1, 1 },
                    { 257, 31, 50, 0, 0, 0 },
                    { 256, 30, 10000, 5001, 12, 10 },
                    { 255, 30, 5000, 2501, 10, 8 },
                    { 230, 27, 5000, 2501, 5, 4 },
                    { 252, 30, 500, 141, 4, 4 },
                    { 229, 27, 2500, 1001, 4, 3 },
                    { 226, 27, 100, 51, 1, 1 },
                    { 227, 27, 300, 101, 2, 2 },
                    { 203, 24, 50, 0, 0, 0 },
                    { 202, 23, 10000, 5001, 12, 10 },
                    { 201, 23, 5000, 2501, 10, 8 },
                    { 200, 23, 2500, 1001, 8, 7 },
                    { 199, 23, 1000, 501, 6, 5 },
                    { 198, 23, 500, 301, 4, 4 },
                    { 197, 23, 300, 101, 4, 3 },
                    { 196, 23, 100, 51, 2, 2 },
                    { 195, 23, 50, 0, 0, 0 },
                    { 194, 22, 10000, 5001, 12, 10 },
                    { 193, 22, 5000, 2501, 10, 8 },
                    { 192, 22, 2500, 1001, 8, 7 },
                    { 191, 22, 1000, 501, 6, 5 },
                    { 190, 22, 500, 301, 4, 4 },
                    { 189, 22, 300, 101, 4, 3 },
                    { 188, 22, 100, 51, 2, 2 },
                    { 187, 22, 50, 20, 1, 1 },
                    { 186, 22, 19, 0, 0, 0 },
                    { 185, 21, 10000, 5001, 12, 9 },
                    { 204, 24, 100, 51, 3, 3 },
                    { 228, 27, 1000, 301, 3, 3 },
                    { 205, 24, 300, 101, 4, 3 },
                    { 207, 24, 1000, 501, 6, 5 },
                    { 273, 33, 100, 51, 1, 1 },
                    { 225, 27, 50, 0, 0, 0 },
                    { 224, 26, 10000, 5001, 8, 6 },
                    { 223, 26, 5000, 2501, 6, 5 },
                    { 222, 26, 2500, 501, 5, 4 },
                    { 221, 26, 500, 301, 4, 3 },
                    { 220, 26, 300, 81, 3, 3 },
                    { 219, 26, 80, 30, 1, 1 },
                    { 218, 26, 29, 0, 0, 0 },
                    { 217, 25, 10000, 5001, 6, 4 },
                    { 216, 25, 5000, 2501, 5, 4 },
                    { 215, 25, 2500, 1001, 4, 3 },
                    { 214, 25, 1000, 301, 3, 3 },
                    { 213, 25, 300, 101, 2, 2 },
                    { 212, 25, 100, 51, 1, 1 },
                    { 211, 25, 50, 0, 0, 0 },
                    { 210, 24, 10000, 5001, 15, 12 },
                    { 209, 24, 5000, 2501, 12, 9 },
                    { 208, 24, 2500, 1001, 9, 7 },
                    { 206, 24, 500, 301, 4, 4 },
                    { 274, 33, 140, 101, 2, 1 },
                    { 277, 33, 2500, 1001, 5, 5 },
                    { 276, 33, 1000, 501, 4, 4 },
                    { 341, 42, 1000, 301, 3, 3 },
                    { 340, 42, 300, 101, 2, 2 },
                    { 339, 42, 100, 51, 1, 1 },
                    { 338, 42, 50, 0, 0, 0 },
                    { 337, 41, 10000, 5001, 6, 5 },
                    { 336, 41, 5000, 2501, 5, 4 },
                    { 335, 41, 2500, 1001, 4, 3 },
                    { 334, 41, 1000, 301, 3, 3 },
                    { 333, 41, 300, 101, 2, 2 },
                    { 332, 41, 100, 51, 1, 1 },
                    { 331, 41, 50, 0, 0, 0 },
                    { 330, 40, 10000, 5001, 10, 9 },
                    { 329, 40, 5000, 2501, 9, 8 },
                    { 328, 40, 2500, 1001, 8, 7 },
                    { 327, 40, 1000, 501, 7, 6 },
                    { 326, 40, 500, 301, 5, 4 },
                    { 325, 40, 300, 141, 4, 4 },
                    { 324, 40, 140, 101, 4, 3 },
                    { 323, 40, 100, 81, 2, 2 },
                    { 342, 42, 2500, 1001, 4, 3 },
                    { 322, 40, 80, 20, 1, 1 },
                    { 343, 42, 5000, 2501, 5, 4 },
                    { 345, 43, 100, 0, 0, 0 },
                    { 364, 45, 2500, 1001, 4, 3 },
                    { 363, 45, 1000, 501, 3, 3 },
                    { 362, 45, 500, 101, 2, 2 },
                    { 361, 45, 100, 51, 1, 1 },
                    { 360, 45, 50, 0, 0, 0 },
                    { 359, 44, 10000, 5001, 12, 9 },
                    { 358, 44, 5000, 2501, 10, 8 },
                    { 357, 44, 2500, 1001, 8, 7 },
                    { 356, 44, 1000, 501, 6, 5 },
                    { 355, 44, 500, 301, 4, 4 },
                    { 354, 44, 300, 101, 4, 3 },
                    { 353, 44, 100, 51, 2, 2 },
                    { 352, 44, 50, 20, 1, 1 },
                    { 351, 44, 19, 0, 0, 0 },
                    { 350, 43, 10000, 5001, 5, 4 },
                    { 349, 43, 5000, 2501, 4, 3 },
                    { 348, 43, 2500, 1001, 3, 3 },
                    { 347, 43, 1000, 501, 2, 2 },
                    { 346, 43, 500, 101, 1, 1 },
                    { 344, 42, 10000, 5001, 6, 5 },
                    { 275, 33, 500, 141, 2, 2 },
                    { 321, 40, 19, 0, 0, 0 },
                    { 319, 39, 5000, 2501, 4, 3 },
                    { 295, 36, 300, 0, 0, 0 },
                    { 294, 35, 10000, 5001, 6, 5 },
                    { 293, 35, 5000, 2501, 5, 4 },
                    { 292, 35, 2500, 1001, 4, 3 },
                    { 291, 35, 1000, 501, 3, 3 },
                    { 290, 35, 500, 101, 2, 2 },
                    { 289, 35, 100, 51, 1, 1 },
                    { 288, 35, 50, 0, 0, 0 },
                    { 287, 34, 10000, 5001, 9, 9 },
                    { 286, 34, 5000, 2501, 7, 7 },
                    { 285, 34, 2500, 1001, 5, 5 },
                    { 284, 34, 1000, 501, 4, 4 },
                    { 283, 34, 500, 301, 3, 2 },
                    { 282, 34, 300, 141, 2, 2 },
                    { 281, 34, 140, 101, 2, 1 },
                    { 280, 34, 100, 51, 1, 1 },
                    { 279, 34, 50, 0, 0, 0 },
                    { 278, 33, 10000, 2501, 7, 7 },
                    { 184, 21, 5000, 2501, 10, 7 },
                    { 296, 36, 500, 301, 1, 1 },
                    { 320, 39, 10000, 5001, 5, 4 },
                    { 297, 36, 1000, 501, 2, 2 },
                    { 299, 36, 5000, 2501, 4, 3 },
                    { 318, 39, 2500, 1001, 3, 3 },
                    { 317, 39, 1000, 501, 2, 2 },
                    { 316, 39, 500, 301, 1, 1 },
                    { 315, 39, 300, 0, 0, 0 },
                    { 314, 38, 10000, 2501, 6, 5 },
                    { 313, 38, 2500, 1001, 5, 5 },
                    { 312, 38, 1000, 501, 4, 4 },
                    { 311, 38, 500, 301, 3, 3 },
                    { 310, 38, 300, 141, 2, 2 },
                    { 309, 38, 140, 101, 1, 1 },
                    { 308, 38, 100, 0, 0, 0 },
                    { 307, 37, 10000, 2501, 6, 5 },
                    { 306, 37, 2500, 1001, 5, 4 },
                    { 305, 37, 1000, 501, 4, 3 },
                    { 304, 37, 500, 301, 3, 3 },
                    { 303, 37, 300, 141, 2, 2 },
                    { 302, 37, 140, 101, 1, 1 },
                    { 301, 37, 100, 0, 0, 0 },
                    { 300, 36, 10000, 5001, 5, 4 },
                    { 298, 36, 2500, 1001, 3, 3 },
                    { 183, 21, 2500, 1001, 8, 6 },
                    { 179, 21, 140, 81, 3, 3 },
                    { 181, 21, 500, 301, 5, 4 },
                    { 65, 8, 5000, 2501, 6, 4 },
                    { 64, 8, 2500, 1001, 4, 3 },
                    { 63, 8, 1000, 301, 3, 3 },
                    { 62, 8, 300, 101, 2, 2 },
                    { 61, 8, 100, 51, 1, 1 },
                    { 60, 8, 50, 0, 0, 0 },
                    { 59, 7, 10000, 5001, 11, 9 },
                    { 58, 7, 5000, 1001, 9, 7 },
                    { 57, 7, 1000, 501, 6, 5 },
                    { 56, 7, 500, 141, 4, 4 },
                    { 55, 7, 140, 121, 4, 3 },
                    { 54, 7, 120, 81, 3, 3 },
                    { 53, 7, 80, 51, 2, 2 },
                    { 52, 7, 50, 20, 1, 1 },
                    { 51, 7, 19, 0, 0, 0 },
                    { 50, 6, 10000, 5001, 6, 4 },
                    { 49, 6, 5000, 2501, 5, 4 },
                    { 48, 6, 2500, 1001, 3, 3 },
                    { 47, 6, 1000, 141, 2, 2 },
                    { 66, 8, 10000, 5001, 7, 5 },
                    { 46, 6, 140, 30, 1, 1 },
                    { 67, 9, 19, 0, 0, 0 },
                    { 69, 9, 80, 51, 2, 2 },
                    { 88, 11, 300, 141, 4, 3 },
                    { 87, 11, 140, 101, 3, 3 },
                    { 86, 11, 100, 51, 2, 2 },
                    { 85, 11, 50, 20, 1, 1 },
                    { 84, 11, 19, 0, 0, 0 },
                    { 83, 10, 10000, 5001, 6, 4 },
                    { 82, 10, 5000, 2501, 5, 4 },
                    { 81, 10, 2500, 1001, 4, 3 },
                    { 80, 10, 1000, 501, 3, 3 },
                    { 79, 10, 500, 101, 2, 2 },
                    { 78, 10, 100, 51, 1, 1 },
                    { 77, 10, 50, 0, 0, 0 },
                    { 76, 9, 10000, 5001, 12, 10 },
                    { 75, 9, 5000, 2501, 10, 8 },
                    { 74, 9, 2500, 1001, 8, 6 },
                    { 73, 9, 1000, 501, 6, 4 },
                    { 72, 9, 500, 141, 5, 4 },
                    { 71, 9, 140, 121, 4, 3 },
                    { 70, 9, 120, 81, 3, 3 },
                    { 68, 9, 50, 20, 1, 1 },
                    { 89, 11, 500, 301, 5, 4 },
                    { 45, 6, 29, 0, 0, 0 },
                    { 43, 5, 5000, 2501, 5, 4 },
                    { 19, 3, 50, 20, 1, 1 },
                    { 18, 3, 19, 0, 0, 0 },
                    { 17, 2, 10000, 5001, 15, 12 },
                    { 16, 2, 5000, 2501, 12, 9 },
                    { 15, 2, 2500, 1001, 9, 8 },
                    { 14, 2, 1000, 501, 6, 5 },
                    { 13, 2, 500, 301, 4, 4 },
                    { 12, 2, 300, 101, 4, 3 },
                    { 11, 2, 100, 51, 3, 3 },
                    { 10, 2, 50, 20, 1, 1 },
                    { 9, 2, 19, 0, 0, 0 },
                    { 8, 1, 10000, 5001, 15, 12 },
                    { 7, 1, 5000, 2501, 12, 9 },
                    { 6, 1, 2500, 1001, 9, 7 },
                    { 5, 1, 1000, 501, 6, 4 },
                    { 4, 1, 500, 101, 4, 3 },
                    { 3, 1, 100, 51, 3, 3 },
                    { 2, 1, 50, 20, 1, 1 },
                    { 1, 1, 19, 0, 0, 0 },
                    { 20, 3, 100, 51, 2, 2 },
                    { 44, 5, 10000, 5001, 6, 5 },
                    { 21, 3, 120, 101, 3, 3 },
                    { 23, 3, 300, 141, 4, 4 },
                    { 42, 5, 2500, 1001, 4, 3 },
                    { 41, 5, 1000, 301, 3, 3 },
                    { 40, 5, 300, 101, 2, 2 },
                    { 39, 5, 100, 51, 1, 1 },
                    { 38, 5, 50, 0, 0, 0 },
                    { 37, 4, 10000, 2501, 10, 8 },
                    { 36, 4, 2500, 1001, 7, 6 },
                    { 35, 4, 1000, 501, 6, 5 },
                    { 34, 4, 500, 301, 5, 4 },
                    { 33, 4, 300, 141, 4, 4 },
                    { 32, 4, 140, 101, 3, 3 },
                    { 31, 4, 100, 51, 2, 2 },
                    { 30, 4, 50, 20, 1, 1 },
                    { 29, 4, 19, 0, 0, 0 },
                    { 28, 3, 10000, 5001, 11, 9 },
                    { 27, 3, 5000, 2501, 10, 7 },
                    { 26, 3, 2500, 1001, 7, 6 },
                    { 25, 3, 1000, 501, 6, 5 },
                    { 24, 3, 500, 301, 5, 4 },
                    { 22, 3, 140, 121, 4, 3 },
                    { 182, 21, 1000, 501, 6, 4 },
                    { 90, 11, 1000, 501, 6, 5 },
                    { 92, 11, 5000, 2501, 9, 8 },
                    { 157, 18, 1000, 501, 6, 5 },
                    { 156, 18, 500, 301, 5, 4 },
                    { 155, 18, 300, 141, 4, 4 },
                    { 154, 18, 140, 121, 4, 3 },
                    { 153, 18, 120, 101, 3, 3 },
                    { 152, 18, 100, 51, 2, 2 },
                    { 151, 18, 50, 20, 1, 1 },
                    { 150, 18, 19, 0, 0, 0 },
                    { 149, 17, 10000, 5001, 13, 10 },
                    { 148, 17, 5000, 2501, 11, 8 },
                    { 147, 17, 2500, 1001, 9, 7 },
                    { 146, 17, 1000, 501, 6, 5 },
                    { 145, 17, 500, 301, 5, 4 },
                    { 144, 17, 300, 141, 4, 3 },
                    { 143, 17, 140, 51, 3, 3 },
                    { 142, 17, 50, 20, 1, 1 },
                    { 141, 17, 19, 0, 0, 0 },
                    { 140, 16, 10000, 5001, 10, 8 },
                    { 139, 16, 5000, 2501, 9, 7 },
                    { 158, 18, 2500, 1001, 9, 7 },
                    { 138, 16, 2500, 1001, 8, 6 },
                    { 159, 18, 10000, 2501, 11, 9 },
                    { 161, 19, 100, 51, 1, 1 },
                    { 180, 21, 300, 141, 4, 3 },
                    { 365, 45, 5000, 2501, 5, 4 },
                    { 178, 21, 80, 51, 2, 2 },
                    { 177, 21, 50, 20, 1, 1 },
                    { 176, 21, 19, 0, 0, 0 },
                    { 175, 20, 10000, 5001, 12, 10 },
                    { 174, 20, 5000, 2501, 10, 8 },
                    { 173, 20, 2500, 1001, 8, 6 },
                    { 172, 20, 1000, 501, 6, 4 },
                    { 171, 20, 500, 301, 5, 4 },
                    { 170, 20, 300, 101, 4, 3 },
                    { 169, 20, 100, 51, 3, 3 },
                    { 168, 20, 50, 20, 1, 1 },
                    { 167, 20, 19, 0, 0, 0 },
                    { 166, 19, 10000, 5001, 6, 4 },
                    { 165, 19, 5000, 2501, 5, 4 },
                    { 164, 19, 2500, 1001, 4, 3 },
                    { 163, 19, 1000, 301, 3, 3 },
                    { 162, 19, 300, 101, 2, 2 },
                    { 160, 19, 50, 0, 0, 0 },
                    { 91, 11, 2500, 1001, 8, 7 },
                    { 137, 16, 1000, 501, 7, 6 },
                    { 135, 16, 300, 121, 4, 3 },
                    { 111, 14, 19, 0, 0, 0 },
                    { 110, 13, 10000, 5001, 7, 5 },
                    { 109, 13, 5000, 2501, 6, 4 },
                    { 108, 13, 2500, 1001, 5, 4 },
                    { 107, 13, 1000, 501, 3, 3 },
                    { 106, 13, 500, 121, 2, 2 },
                    { 105, 13, 120, 51, 1, 1 },
                    { 104, 13, 50, 0, 0, 0 },
                    { 103, 12, 10000, 5001, 10, 8 },
                    { 102, 12, 5000, 2501, 8, 6 },
                    { 101, 12, 2500, 1001, 7, 5 },
                    { 100, 12, 1000, 501, 6, 4 },
                    { 99, 12, 500, 301, 5, 4 },
                    { 98, 12, 300, 141, 4, 3 },
                    { 97, 12, 140, 101, 3, 3 },
                    { 96, 12, 100, 51, 2, 2 },
                    { 95, 12, 50, 20, 1, 1 },
                    { 94, 12, 19, 0, 0, 0 },
                    { 93, 11, 10000, 5001, 10, 8 }
                });

            migrationBuilder.InsertData(
                table: "LinhasDimensionamentos",
                columns: new[] { "Id", "GrupoId", "Maximo", "Minimo", "QtdaEfetivos", "QtdaSuplentes" },
                values: new object[,]
                {
                    { 112, 14, 50, 20, 1, 1 },
                    { 136, 16, 500, 301, 5, 4 },
                    { 113, 14, 100, 51, 2, 2 },
                    { 115, 14, 300, 141, 4, 3 },
                    { 134, 16, 120, 81, 3, 3 },
                    { 133, 16, 80, 51, 2, 2 },
                    { 132, 16, 50, 20, 1, 1 },
                    { 131, 16, 19, 0, 0, 0 },
                    { 130, 15, 10000, 5001, 12, 10 },
                    { 129, 15, 5000, 2501, 10, 8 },
                    { 128, 15, 2500, 1001, 9, 7 },
                    { 127, 15, 1000, 501, 6, 4 },
                    { 114, 14, 140, 101, 3, 3 },
                    { 126, 15, 500, 301, 5, 4 },
                    { 124, 15, 120, 81, 3, 3 },
                    { 123, 15, 80, 51, 2, 2 },
                    { 122, 15, 50, 20, 1, 1 },
                    { 121, 15, 19, 0, 0, 0 },
                    { 120, 14, 10000, 5001, 10, 8 },
                    { 119, 14, 5000, 2501, 9, 7 },
                    { 118, 14, 2500, 1001, 8, 6 },
                    { 117, 14, 1000, 501, 5, 4 },
                    { 116, 14, 500, 301, 4, 4 },
                    { 125, 15, 300, 121, 4, 3 },
                    { 366, 45, 10000, 5001, 6, 5 }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Cargo", "CodigoRecuperacao", "ContaId", "DataCadastro", "Email", "ExpiracaoCodigoRecuperacao", "Nome", "Perfil", "Senha" },
                values: new object[] { 1, null, null, 1, new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "teste@email.com", null, "Teste", "SESMT", "03c32dc379d1b0958f3ef87d94ebb4ec859b9e2fdd297f44d68d8dd5f36800cc" });

            migrationBuilder.InsertData(
                table: "Estabelecimentos",
                columns: new[] { "Id", "Ativo", "Cidade", "DataCadastro", "Descricao", "EmpresaId", "Endereco", "GrupoId" },
                values: new object[] { 1, true, "Uberlândia", new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Teste", 1, "Teste", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Contas_PlanoId",
                table: "Contas",
                column: "PlanoId");

            migrationBuilder.CreateIndex(
                name: "IX_Eleicoes_ContaId",
                table: "Eleicoes",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_Eleicoes_EstabelecimentoId",
                table: "Eleicoes",
                column: "EstabelecimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Eleicoes_GrupoId",
                table: "Eleicoes",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_Eleicoes_UsuarioCriacaoId",
                table: "Eleicoes",
                column: "UsuarioCriacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Eleitores_EleicaoId",
                table: "Eleitores",
                column: "EleicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Eleitores_UsuarioId",
                table: "Eleitores",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_ContaId",
                table: "Empresas",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_Estabelecimentos_EmpresaId",
                table: "Estabelecimentos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Estabelecimentos_GrupoId",
                table: "Estabelecimentos",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_EtapasCronogramas_EleicaoId",
                table: "EtapasCronogramas",
                column: "EleicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_EtapasCronogramas_EtapaObrigatoriaId",
                table: "EtapasCronogramas",
                column: "EtapaObrigatoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_EtapasPadroesContas_ContaId",
                table: "EtapasPadroesContas",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_EtapasPadroesContas_EtapaObrigatoriaId",
                table: "EtapasPadroesContas",
                column: "EtapaObrigatoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscricoes_EleicaoId",
                table: "Inscricoes",
                column: "EleicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscricoes_EleitorId",
                table: "Inscricoes",
                column: "EleitorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LinhasDimensionamentos_GrupoId",
                table: "LinhasDimensionamentos",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reprovacoes_InscricaoId",
                table: "Reprovacoes",
                column: "InscricaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_ContaId",
                table: "Usuarios",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_EleicaoId",
                table: "Votos",
                column: "EleicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_EleitorId",
                table: "Votos",
                column: "EleitorId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EtapasCronogramas");

            migrationBuilder.DropTable(
                name: "EtapasPadroesContas");

            migrationBuilder.DropTable(
                name: "LinhasDimensionamentos");

            migrationBuilder.DropTable(
                name: "Reprovacoes");

            migrationBuilder.DropTable(
                name: "Votos");

            migrationBuilder.DropTable(
                name: "EtapasObrigatorias");

            migrationBuilder.DropTable(
                name: "Inscricoes");

            migrationBuilder.DropTable(
                name: "Eleitores");

            migrationBuilder.DropTable(
                name: "Eleicoes");

            migrationBuilder.DropTable(
                name: "Estabelecimentos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "Grupos");

            migrationBuilder.DropTable(
                name: "Contas");

            migrationBuilder.DropTable(
                name: "Plano");
        }
    }
}
