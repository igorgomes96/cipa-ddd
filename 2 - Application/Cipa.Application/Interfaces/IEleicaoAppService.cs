using Cipa.Domain.Entities;
using System.Collections.Generic;
using System.IO;

namespace Cipa.Application.Interfaces
{
    public interface IEleicaoAppService : IAppServiceBase<Eleicao>
    {
        IEnumerable<Eleicao> BuscarPelaConta(int contaId);
        IEnumerable<Eleicao> BuscarPeloUsuario(int usuarioId);
        IEnumerable<EtapaCronograma> BuscarCronograma(int eleicaoId);
        IEnumerable<EtapaCronograma> AtualizarCronograma(int eleicaoId, EtapaCronograma etapa);
        IEnumerable<Inscricao> BuscarInscricoes(int eleicaoId, StatusInscricao? status);
        Inscricao BuscarInscricaoPeloUsuario(int eleicaoId, int usuarioId);
        IEnumerable<Eleitor> BuscarEleitores(int eleicaoId);
        Eleitor BuscarEleitor(int eleicaoId, int eleitorId);
        void ExcluirTodosEleitores(int eleicaoId);
        Eleitor ExcluirEleitor(int eleicaoId, int eleitorId);
        Eleitor AtualizarEleitor(int eleicaoId, Eleitor eleitor);
        Eleitor BuscarEleitorPeloIdUsuario(int eleicaoId, int usuarioId);
        Eleicao PassarParaProximaEtapa(int eleicaoId);
        Eleitor AdicionarEleitor(int eleicaoId, Eleitor eleitor);
        Inscricao FazerInscricao(int eleicaoId, int usuarioId, string objetivos);
        Inscricao AtualizarInscricao(int eleicaoId, int usuarioId, string objetivos);
        Inscricao AprovarInscricao(int eleicaoId, int inscricaoId, int usuarioId);
        Inscricao ReprovarInscricao(int eleicaoId, int inscricaoId, int usuarioId, string motivoReprovacao);
        Voto RegistrarVoto(int eleicaoId, int inscricaoId, int usuarioId, string ip);
        Voto VotarEmBranco(int eleicaoId, int usuarioId, string ip);
        IEnumerable<Voto> BuscarVotos(int eleicaoId);
        Voto BuscarVotoUsuario(int eleicaoId, int usuarioId);
        IEnumerable<Inscricao> ApurarVotos(int eleicaoId);
        Inscricao AtualizarFotoInscricao(int eleicaoId, int usuarioId, byte[] foto, string fotoFileName);
        Stream BuscarFotoInscricao(int eleicaoId, int inscricaoId);
        Importacao ImportarEleitores(int eleicaoId, int usuarioId, byte[] conteudoArquivo, string nomeArquivo, string contentType);
        Arquivo FazerUploadArquivo(int eleicaoId, int etapaId, int usuarioId, byte[] conteudoArquivo, string nomeArquivo, string contentType);
        bool VerificarSeUsuarioEhEleitor(int eleicaoId, int usuarioId);
        Stream GerarRelatorioInscricoes(int eleicaoId);
        Stream GerarRelatorioVotos(int eleicaoId);
    }
}