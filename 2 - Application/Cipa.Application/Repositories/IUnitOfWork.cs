using System;
namespace Cipa.Application.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IContaRepository ContaRepository { get; }
        IEleicaoRepository EleicaoRepository { get; }
        IGrupoRepository GrupoRepository { get; }
        IEstabelecimentoRepository EstabelecimentoRepository { get; }
        IUsuarioRepository UsuarioRepository { get; }
        IEmpresaRepository EmpresaRepository { get; }
        IImportacaoRepository ImportacaoRepository { get; }
        IArquivoRepository ArquivoRepository { get; }
        IEmailRepository EmailRepository { get; }
        IProcessamentoEtapaRepository ProcessamentoEtapaRepository { get; }
        void Commit();
        void Rollback();
    }
}