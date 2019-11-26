using System;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Infra.Data.Context;

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
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    dbContextTransaction.Rollback();
                    throw;
                }
            }

        }

        #region Public Properties  

        // private IContaRepository _contaRepository;
        // public IContaRepository ContaRepository => _contaRepository ?? (_contaRepository = (IContaRepository)_serviceProvider.GetService(typeof(IContaRepository)));
        public IContaRepository ContaRepository => (IContaRepository)_serviceProvider.GetService(typeof(IContaRepository));
        public IEleicaoRepository EleicaoRepository => (IEleicaoRepository)_serviceProvider.GetService(typeof(IEleicaoRepository));
        public IGrupoRepository GrupoRepository => (IGrupoRepository)_serviceProvider.GetService(typeof(IGrupoRepository));
        public IEstabelecimentoRepository EstabelecimentoRepository => (IEstabelecimentoRepository)_serviceProvider.GetService(typeof(IEstabelecimentoRepository));
        public IUsuarioRepository UsuarioRepository => (IUsuarioRepository)_serviceProvider.GetService(typeof(IUsuarioRepository));
        public IEmpresaRepository EmpresaRepository => (IEmpresaRepository)_serviceProvider.GetService(typeof(IEmpresaRepository));

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