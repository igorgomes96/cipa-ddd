using System.Collections.Generic;
using System.Threading.Tasks;
using Cipa.Domain.Entities;

namespace Cipa.Application.Interfaces
{
    public interface IImportacaoAppService
    {
        Task RealizarImportacaoEmBrackground();
        Importacao RetornarUltimaImportacaoDaEleicao(int eleicaoId);
        IEnumerable<Inconsistencia> RetornarInconsistenciasDaImportacao(int id);
    }
}
