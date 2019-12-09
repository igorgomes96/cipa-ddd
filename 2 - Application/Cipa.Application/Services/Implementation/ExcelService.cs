using Cipa.Domain.Exceptions;
using Cipa.Application.Services.Interfaces;
using ClosedXML.Excel;
using System.Data;
using System.IO;
using System.Linq;

namespace Cipa.Application.Services.Implementation
{
    public class ExcelService : IExcelService
    {
        public DataTable LerTabela(string fileName, int colunaInicial, int colunaFinal, int linhaInicial = 1, bool temCabecalho = true)
        {
            if (!File.Exists(fileName)) throw new CustomException("Arquivo não encontrado.");
            string[] extensoesSuportadas = new string[] { ".xlsx", ".xlsm", ".xltx", ".xltm" };
            if (!extensoesSuportadas.Contains(Path.GetExtension(fileName)))
            {
                var extensoes = extensoesSuportadas.Aggregate((pre, cur) =>
                {
                    return $"{pre}{cur}, ";
                });
                extensoes = extensoes.Substring(0, extensoes.Length - 1);
                throw new CustomException($"A extensão do arquivo é inválida. Somente as extensões {extensoes} são suportadas.");
            }
            var dataTable = new DataTable();
            using (var workbook = new XLWorkbook(fileName))
            {
                var sheet = workbook.Worksheet(1);
                var row = sheet.Row(linhaInicial);

                // Cabeçalho
                if (temCabecalho)
                {
                    for (int c = colunaInicial; c <= colunaFinal; c++)
                    {
                        dataTable.Columns.Add(row.Cell(c).GetString());
                    }
                    row = row.RowBelow();
                }
                else
                {
                    for (int c = colunaInicial; c <= colunaFinal; c++)
                        dataTable.Columns.Add($"Column{c}");
                }

                while (!row.IsEmpty())
                {
                    DataRow dr = dataTable.NewRow();
                    for (int c = colunaInicial; c <= colunaFinal; c++)
                        dr[c - colunaInicial] = row.Cell(c).Value;

                    dataTable.Rows.Add(dr);
                    row = row.RowBelow();
                }
            }
            return dataTable;
        }

        public void GravaInformacoesArquivo(string arquivo, DataTable dataTable, string nome, int linha, int coluna)
        {
            if (File.Exists(arquivo))
            {
                throw new CustomException($"O arquivo {arquivo} já existe.");
            }
            if (dataTable.Rows.Count == 0) return;
            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.AddWorksheet(nome);

                var column = 1;
                foreach (DataColumn dtColumn in dataTable.Columns)
                    sheet.Cell(linha, column++).Value = dtColumn.ColumnName;

                linha++;
                var columnsCount = dataTable.Columns.Count;
                foreach (DataRow dr in dataTable.Rows)
                {
                    for (int c = 0; c < columnsCount; c++)
                        sheet.Cell(linha, c + coluna).Value = dr[c];

                    linha++;
                }

                workbook.SaveAs(arquivo);
            }
        }
    }
}