using System;
using System.Collections.Generic;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Domain.Interfaces.Services;

namespace Cipa.Domain.Services
{
    public class ServiceBase<TEntity> : IDisposable, IServiceBase<TEntity> where TEntity : class
    {
        protected readonly IRepositoryBase<TEntity> _repository;
        protected readonly IUnitOfWork _unitOfWork;

        public ServiceBase(IRepositoryBase<TEntity> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public virtual TEntity Adicionar(TEntity obj)
        {
            TEntity entity = _repository.Adicionar(obj);
            _unitOfWork.Commit();
            return entity;
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
            _unitOfWork.Commit();
        }

        public virtual TEntity Atualizar(TEntity obj)
        {
            _repository.Atualizar(obj);
            _unitOfWork.Commit();
            return obj;
        }

        public void Dispose()
        {
            _repository.Dispose();
            _unitOfWork.Dispose();
        }
    }
}