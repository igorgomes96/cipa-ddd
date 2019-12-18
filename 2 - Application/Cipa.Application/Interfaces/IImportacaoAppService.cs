using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Application.Interfaces
{
    public interface IImportacaoAppService
    {
        void RealizarImportacaoEmBrackground();
        Importacao RetornarUltimaImportacaoDaEleicao(int eleicaoId);
        IEnumerable<Inconsistencia> RetornarInconsistenciasDaImportacao(int id);
    }
}
