using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Domain.Interfaces.Services {
    public interface IEleicaoService: IServiceBase<Eleicao>
    {
        Eleicao BuscarPeloIdCarregarEleitores(int id);
        IEnumerable<Inscricao> BuscarInscricoes(int id, StatusInscricao? status = null);
        IEnumerable<Eleitor> BuscarEleitores(int id);
        Inscricao BuscarInscricaoPeloUsuario(int eleicaoId, Usuario usuario);
        IEnumerable<Eleicao> BuscarPelaConta(Conta conta);
        IEnumerable<Eleicao> BuscarPeloUsuario(Usuario usuario);
        Dimensionamento CalcularDimensionamentoEleicao(Eleicao eleicao);
        bool ExcluirEleitor(Eleicao eleicao, int eleitorId);
        Eleicao PassarParaProximaEtapa(Eleicao eleicao);
        Eleicao BuscarPeloIdCarregarTodoAgregado(int id);
        Eleitor AdicionarEleitor(Eleicao eleicao, Eleitor eleitor);
        Inscricao FazerInscricao(Eleicao eleicao, Eleitor eleitor, string objetivos);
        Inscricao AtualizarInscricao(Eleicao eleicao, Eleitor eleitor, string objetivos);
        Inscricao AprovarInscricao(Eleicao eleicao, int inscricaoId, Usuario usuarioAprovador);
        Inscricao ReprovarInscricao(Eleicao eleicao, int inscricaoId, Usuario usuarioAprovador, string motivoReprovacao);
    }
}