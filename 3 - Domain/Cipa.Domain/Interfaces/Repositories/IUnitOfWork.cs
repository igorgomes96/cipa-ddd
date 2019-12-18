using System;
namespace Cipa.Domain.Interfaces.Repositories
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
        void Commit();
    }
}