using System.Data;

namespace Cipa.Services.Interfaces {
    public interface IExcelService
    {
        DataTable LerTabela(string fileName, int colunaInicial, int colunaFinal, int linhaInicial = 1, bool temCabecalho = true);
        void GravaInformacoesArquivo(string arquivo, DataTable dataTable, string nome, int linha, int coluna);
    }
}