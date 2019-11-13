using System;
using System.Collections.Generic;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Domain.Interfaces.Services;

namespace Cipa.Domain.Services {
    public class ServiceBase<TEntity> : IDisposable, IServiceBase<TEntity> where TEntity : class
    {
        protected readonly IRepositoryBase<TEntity> _repository;

        public ServiceBase(IRepositoryBase<TEntity> repository) {
            _repository = repository;
        }

        public virtual TEntity Adicionar(TEntity obj)
        {
            return _repository.Adicionar(obj);
        }

        public virtual IEnumerable<TEntity> BuscarTodos()
        {
            return _repository.BuscarTodos();
        }

        public virtual TEntity BuscarPeloId(int id)
        {
            return _repository.BuscarPeloId(id);
        }

        public virtual void Excluir(TEntity obj)
        {
            _repository.Excluir(obj);
        }

        public virtual void Atualizar(TEntity obj)
        {
            _repository.Atualizar(obj);
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}