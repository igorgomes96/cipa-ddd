using Cipa.Application.Repositories;
using Cipa.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Cipa.Infra.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly CipaContext Context;
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWork(IServiceProvider serviceProvider, CipaContext context)
        {
            Context = context;
            _serviceProvider = serviceProvider;
        }

        public void Commit()
        {
            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    Context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    //Log Exception Handling message                      
                    dbContextTransaction.Rollback();
                    throw;
                }
            }

        }

        public void Rollback()
        {
            var changedEntries = Context.ChangeTracker.Entries()
                .Where(x => x.State != EntityState.Unchanged).ToList();

            foreach (var entry in changedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }
        }
        #region Public Properties  

        public IContaRepository ContaRepository => (IContaRepository)_serviceProvider.GetService(typeof(IContaRepository));
        public IEleicaoRepository EleicaoRepository => (IEleicaoRepository)_serviceProvider.GetService(typeof(IEleicaoRepository));
        public IGrupoRepository GrupoRepository => (IGrupoRepository)_serviceProvider.GetService(typeof(IGrupoRepository));
        public IEstabelecimentoRepository EstabelecimentoRepository => (IEstabelecimentoRepository)_serviceProvider.GetService(typeof(IEstabelecimentoRepository));
        public IUsuarioRepository UsuarioRepository => (IUsuarioRepository)_serviceProvider.GetService(typeof(IUsuarioRepository));
        public IEmpresaRepository EmpresaRepository => (IEmpresaRepository)_serviceProvider.GetService(typeof(IEmpresaRepository));
        public IImportacaoRepository ImportacaoRepository => (IImportacaoRepository)_serviceProvider.GetService(typeof(IImportacaoRepository));
        public IArquivoRepository ArquivoRepository => (IArquivoRepository)_serviceProvider.GetService(typeof(IArquivoRepository));
        public IEmailRepository EmailRepository => (IEmailRepository)_serviceProvider.GetService(typeof(IEmailRepository));
        public IProcessamentoEtapaRepository ProcessamentoEtapaRepository => (IProcessamentoEtapaRepository)_serviceProvider.GetService(typeof(IProcessamentoEtapaRepository));

        #endregion


        #region IDisposable Support  
        private bool _disposedValue = false; // To detect redundant calls  

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                //dispose managed state (managed objects).  
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.  
            // set large fields to null.  

            _disposedValue = true;
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.  
        // ~UnitOfWork() {  
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.  
        //   Dispose(false);  
        // }  

        // This code added to correctly implement the disposable pattern.  
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.  
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.  
            // GC.SuppressFinalize(this);  
        }
        #endregion

    }
}