using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cipa.WebApi.Migrations
{
    public partial class Importacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arquivos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Path = table.Column<string>(maxLength: 255, nullable: false),
                    DataCadastro = table.Column<DateTime>(nullable: false),
                    Nome = table.Column<string>(maxLength: 100, nullable: false),
                    Tamanho = table.Column<long>(nullable: false),
                    ContentType = table.Column<string>(maxLength: 255, nullable: false),
                    EmailUsuario = table.Column<string>(maxLength: 100, nullable: false),
                    NomeUsuario = table.Column<string>(maxLength: 255, nullable: false),
                    DependencyType = table.Column<int>(nullable: false),
                    DependencyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arquivos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Importacoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArquivoId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    EleicaoId = table.Column<int>(nullable: false),
                    DataCadastro = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Importacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Importacoes_Arquivos_ArquivoId",
                        column: x => x.ArquivoId,
                        principalTable: "Arquivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Importacoes_Eleicoes_EleicaoId",
                        column: x => x.EleicaoId,
                        principalTable: "Eleicoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inconsistencias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Coluna = table.Column<string>(maxLength: 100, nullable: true),
                    Linha = table.Column<int>(nullable: false),
                    Mensagem = table.Column<string>(maxLength: 255, nullable: true),
                    ImportacaoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inconsistencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inconsistencias_Importacoes_ImportacaoId",
                        column: x => x.ImportacaoId,
                        principalTable: "Importacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Importacoes_ArquivoId",
                table: "Importacoes",
                column: "ArquivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Importacoes_EleicaoId",
                table: "Importacoes",
                column: "EleicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inconsistencias_ImportacaoId",
                table: "Inconsistencias",
                column: "ImportacaoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inconsistencias");

            migrationBuilder.DropTable(
                name: "Importacoes");

            migrationBuilder.DropTable(
                name: "Arquivos");
        }
    }
}
