using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Application.Interfaces {
    public interface IEleicaoAppService: IAppServiceBase<Eleicao>
    {
        IEnumerable<Eleicao> BuscarPelaConta(int contaId);
        IEnumerable<Eleicao> BuscarPeloUsuario(int usuarioId);
        IEnumerable<EtapaCronograma> BuscarCronograma(int eleicaoId);
        IEnumerable<Inscricao> BuscarInscricoes(int eleicaoId, StatusInscricao? status);
        Inscricao BuscarInscricaoPeloUsuario(int eleicaoId, int usuarioId);
        IEnumerable<Eleitor> BuscarEleitores(int eleicaoId);
        bool ExcluirEleitor(int eleicaoId, int eleitorId);
        Eleitor BuscarEleitorPeloIdUsuario(int eleicaoId, int usuarioId);
        Eleicao PassarParaProximaEtapa(int eleicaoId);
        Eleitor AdicionarEleitor(int eleicaoId, Eleitor eleitor);
        Inscricao FazerInscricao(int eleicaoId, int usuarioId, string objetivos);
        Inscricao AtualizarInscricao(int eleicaoId, int usuarioId, string objetivos);
        Inscricao AprovarInscricao(int eleicaoId, int inscricaoId, int usuarioId);
        Inscricao ReprovarInscricao(int eleicaoId, int inscricaoId, int usuarioId, string motivoReprovacao);
    }
}