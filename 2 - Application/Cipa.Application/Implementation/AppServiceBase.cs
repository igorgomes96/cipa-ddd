using System;
using System.Collections.Generic;
using Cipa.Application.Interfaces;
using Cipa.Domain.Interfaces.Services;

namespace Cipa.Application {
    public class AppServiceBase<TEntity> : IDisposable, IAppServiceBase<TEntity> where TEntity : class
    {
        private readonly IServiceBase<TEntity> _serviceBase;

        public AppServiceBase(IServiceBase<TEntity> serviceBase) {
            _serviceBase = serviceBase;
        }

        public virtual TEntity Adicionar(TEntity obj)
        {
            return _serviceBase.Adicionar(obj);
        }

        public virtual IEnumerable<TEntity> BuscarTodos()
        {
            return _serviceBase.BuscarTodos();
        }

        public virtual TEntity BuscarPeloId(int id)
        {
            return _serviceBase.BuscarPeloId(id);
        }

        public virtual void Excluir(TEntity obj)
        {
            _serviceBase.Excluir(obj);
        }

        public virtual void Atualizar(TEntity obj)
        {
            _serviceBase.Atualizar(obj);
        }

        public virtual void Dispose()
        {
            _serviceBase.Dispose();
        }
    }
}