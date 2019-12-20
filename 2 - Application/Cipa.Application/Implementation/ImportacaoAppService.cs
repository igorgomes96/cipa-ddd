using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Cipa.Application.Events;
using Cipa.Application.Events.EventsArgs;
using Cipa.Application.Interfaces;
using Cipa.Application.Services.Interfaces;
using Cipa.Application.Services.Models;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;
using Cipa.Domain.Interfaces.Repositories;

namespace Cipa.Application
{
    public class ImportacaoAppService : AppServiceBase<Importacao>, IImportacaoAppService
    {
        private readonly IExcelService _excelService;
        private readonly DataColumnValidator[] _dataColumnValidators;
        private const int LINHA_INICIAL_ARQUIVO = 1;
        private const int QTDA_MAX_ERROS = 10;
        private const int QTDA_ETAPAS_PPROCESSAMENTO = 3;

        public ImportacaoAppService(
            IUnitOfWork unitOfWork,
            IExcelService excelService,
            IImportacaoServiceConfiguration importacaoConfiguration) : base(unitOfWork, unitOfWork.ImportacaoRepository)
        {
            _excelService = excelService;
            _dataColumnValidators = importacaoConfiguration.Validators;
        }

        private IEnumerable<string> ColunasObrigatoriasNaoEncontradas(DataTable dataTable, IEnumerable<DataColumnValidator> colunasObrigatorias)
        {
            foreach (var column in colunasObrigatorias)
                if (!dataTable.Columns.Contains(column.ColumnName))
                    yield return column.ColumnName;
        }

        private IEnumerable<Inconsistencia> ValidarFormatoDataTable(DataTable dataTable, string emailUsuario)
        {
            var inconsistencias = new List<Inconsistencia>();

            // Valida se possui as colunas obrigatórias
            var obrigatorias = _dataColumnValidators.Where(v => v.Required);
            var colunasNaoEncontradas = ColunasObrigatoriasNaoEncontradas(dataTable, obrigatorias);
            if (colunasNaoEncontradas.Any())
                return colunasNaoEncontradas
                    .Select(coluna => new Inconsistencia(
                        coluna, LINHA_INICIAL_ARQUIVO,
                        $"A coluna {coluna} é obrigatória, porém não foi encontrada no arquivo."));

            // Valida os valores
            int linha = LINHA_INICIAL_ARQUIVO;
            int totalLinhas = dataTable.Rows.Count;
            foreach (DataRow dr in dataTable.Rows)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    var validator = _dataColumnValidators.FirstOrDefault(v => v.ColumnName == dataTable.Columns[i].ColumnName);
                    if (validator == null) continue;
                    object value = dr[validator.ColumnName];
                    var erro = validator.ValidarValor(value);
                    if (!string.IsNullOrWhiteSpace(erro))
                        inconsistencias.Add(new Inconsistencia(validator.ColumnName, linha, erro));
                }
                var countLinhasErros = inconsistencias.Select(i => i.Linha).Distinct().Count();
                if (countLinhasErros > QTDA_MAX_ERROS)
                    break;
                linha++;
                NotificarProgresso(1, linha - LINHA_INICIAL_ARQUIVO, dataTable.Rows.Count, emailUsuario);
            }
            return inconsistencias;

        }

        private T ObtemValorFormatoCorreto<T>(DataRow dr, string columnName, DataColumnValidator validator)
        {
            if (!dr.Table.Columns.Contains(columnName)) return default(T);
            return (T)validator.ParseValor(dr[columnName]);
        }

        private List<Eleitor> ConverterParaListaDeEleitor(DataTable dataTable, string emailUsuario)
        {
            List<Eleitor> eleitores = new List<Eleitor>();
            int totalLinhas = dataTable.Rows.Count;
            int linha = LINHA_INICIAL_ARQUIVO;
            var validators = _dataColumnValidators.ToDictionary(k => k.ColumnName);
            foreach (DataRow dr in dataTable.Rows)
            {
                var nome = ObtemValorFormatoCorreto<string>(dr, ColunasArquivo.Nome, validators[ColunasArquivo.Nome]).Trim();
                var email = ObtemValorFormatoCorreto<string>(dr, ColunasArquivo.Email, validators[ColunasArquivo.Email]).Trim().ToLower();
                var eleitor = new Eleitor(nome, email)
                {
                    Area = ObtemValorFormatoCorreto<string>(dr, ColunasArquivo.Area, validators[ColunasArquivo.Area])?.Trim(),
                    Cargo = ObtemValorFormatoCorreto<string>(dr, ColunasArquivo.Cargo, validators[ColunasArquivo.Cargo])?.Trim(),
                    DataAdmissao = ObtemValorFormatoCorreto<DateTime?>(dr, ColunasArquivo.DataAdmissao, validators[ColunasArquivo.DataAdmissao]),
                    DataNascimento = ObtemValorFormatoCorreto<DateTime?>(dr, ColunasArquivo.DataNascimento, validators[ColunasArquivo.DataNascimento]),
                    Matricula = ObtemValorFormatoCorreto<string>(dr, ColunasArquivo.Matricula, validators[ColunasArquivo.Matricula])?.Trim(),
                    NomeGestor = ObtemValorFormatoCorreto<string>(dr, ColunasArquivo.NomeGestor, validators[ColunasArquivo.NomeGestor])?.Trim(),
                    EmailGestor = ObtemValorFormatoCorreto<string>(dr, ColunasArquivo.EmailGestor, validators[ColunasArquivo.EmailGestor])?.Trim()
                };
                var usuario = _unitOfWork.UsuarioRepository.BuscarUsuario(eleitor.Email);
                if (usuario == null)
                    usuario = new Usuario(eleitor.Email, eleitor.Nome, eleitor.Cargo);
                eleitor.Usuario = usuario;
                eleitores.Add(eleitor);
                linha++;
                NotificarProgresso(2, linha - LINHA_INICIAL_ARQUIVO, dataTable.Rows.Count, emailUsuario);
            }
            return eleitores;
        }

        private IEnumerable<Inconsistencia> RetornarInconsistenciasEmailsDuplicados(List<Eleitor> eleitores) =>
            eleitores.Select(e => e.Email).Distinct()
                .ToDictionary(email => email, email => eleitores.Count(e => e.Email == email))
                .Where(dic => dic.Value > 1)
                .Select(dic => new Inconsistencia(
                    ColunasArquivo.Email, 0,
                    $"Há {dic.Value} linhas no arquivo com o e-mail {dic.Key}."));

        public void RealizarImportacaoEmBrackground()
        {
            Importacao importacao = (_repositoryBase as IImportacaoRepository).BuscarPrimeiraImportacaoPendenteDaFila();
            if (importacao == null) return;
            try
            {
                importacao.IniciarProcessamento();
                base.Atualizar(importacao);
                var arquivoImportacao = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), importacao.Arquivo.Path);
                var dataTable = _excelService.LerTabela(arquivoImportacao, LINHA_INICIAL_ARQUIVO, 10);
                var inconsistencias = ValidarFormatoDataTable(dataTable, importacao.Arquivo.EmailUsuario);

                if (!FinalizarImportacaoComErro(importacao, inconsistencias))
                {
                    var eleitores = ConverterParaListaDeEleitor(dataTable, importacao.Arquivo.EmailUsuario);
                    inconsistencias = RetornarInconsistenciasEmailsDuplicados(eleitores);

                    if (!FinalizarImportacaoComErro(importacao, inconsistencias))
                    {
                        // Busca-se uma eleição no repositório ao invés de utilizar importacao.Eleicao
                        // porque, caso ocorra um erro ao chamar o método SalvarEleitor para algum eleitor da lista,
                        // nenhuma eleitor seja salvo ao atualizar o status da importação para Importação com Erro,
                        // em base.Atualizar(importacao) 
                        var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(importacao.EleicaoId);
                        var linha = 0;
                        foreach (var eleitor in eleitores)
                        {
                            SalvarEleitor(eleicao, eleitor);
                            linha++;
                            NotificarProgresso(3, linha, eleitores.Count, importacao.Arquivo.EmailUsuario);
                        }
                        FinalizarImportacaoComSucesso(importacao);
                        _unitOfWork.EleicaoRepository.Atualizar(eleicao);
                        _unitOfWork.Commit();
                    }
                }

            }
            catch (Exception ex)
            {
                FinalizarImportacaoComErro(importacao, new[] { new Inconsistencia(string.Empty, 0, ex.Message) });
            }
        }

        private bool FinalizarImportacaoComErro(Importacao importacao, IEnumerable<Inconsistencia> inconsistencias)
        {
            if (inconsistencias.Any())
            {
                importacao.FinalizarImportacaoComFalha(inconsistencias);
                base.Atualizar(importacao);
                ProgressoImportacaoEvent.OnImportacaoFinalizada(this,
                    new FinalizacaoImportacaoStatusEventArgs
                    {
                        Status = StatusImportacao.FinalizadoComFalha,
                        QtdaErros = inconsistencias.Count(),
                        EmailUsuario = importacao.Arquivo.EmailUsuario
                    });
                return true;
            }
            return false;
        }

        private void FinalizarImportacaoComSucesso(Importacao importacao)
        {
            importacao.FinalizarProcessamentoComSucesso();
            base.Atualizar(importacao);
            ProgressoImportacaoEvent.OnImportacaoFinalizada(this,
                new FinalizacaoImportacaoStatusEventArgs
                {
                    Status = StatusImportacao.FinalizadoComSucesso,
                    QtdaErros = 0,
                    EmailUsuario = importacao.Arquivo.EmailUsuario
                });
        }

        private void NotificarProgresso(int numEtapa, int linhasProcessadas, int totalLinhas, string emailUsuario)
        {
            ProgressoImportacaoEvent.OnNotificacaoProgresso(this, new ProgressoImportacaoEventArgs
            {
                EtapaAtual = numEtapa,
                LinhasProcessadas = linhasProcessadas,
                TotalEtapas = QTDA_ETAPAS_PPROCESSAMENTO,
                TotalLinhas = totalLinhas,
                EmailUsuario = emailUsuario
            });
        }

        private void SalvarEleitor(Eleicao eleicao, Eleitor eleitor)
        {
            var eleitorCadastrado = eleicao.BuscarEleitorPeloEmail(eleitor.Email);
            if (eleitorCadastrado == null)
                eleicao.AdicionarEleitor(eleitor);
            else
            {
                eleitor.Id = eleitorCadastrado.Id;
                eleicao.AtualizarEleitor(eleitor);
            }
        }

        public IEnumerable<Inconsistencia> RetornarInconsistenciasDaImportacao(int id)
        {
            var importacao = BuscarPeloId(id);
            return importacao.Inconsistencias;
        }

        public Importacao RetornarUltimaImportacaoDaEleicao(int eleicaoId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            return eleicao.Importacoes.OrderBy(i => i.DataCadastro).LastOrDefault();
        }



    }
}