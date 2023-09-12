using Cipa.Application.Interfaces;
using Cipa.Domain.Exceptions;
using Cipa.Application.Repositories;
using System;
using System.Collections.Generic;

namespace Cipa.Application.Implementation
{
    public class AppServiceBase<TEntity> : IDisposable, IAppServiceBase<TEntity> where TEntity : class
    {
        protected readonly IRepositoryBase<TEntity> _repositoryBase;
        protected readonly IUnitOfWork _unitOfWork;

        public AppServiceBase(IUnitOfWork unitOfWork, IRepositoryBase<TEntity> repositoryBase)
        {
            _repositoryBase = repositoryBase;
            _unitOfWork = unitOfWork;
        }

        public virtual TEntity Adicionar(TEntity obj)
        {
            var newObj = _repositoryBase.Adicionar(obj);
            _unitOfWork.Commit();
            return newObj;
        }

        public virtual IEnumerable<TEntity> BuscarTodos()
        {
            return _repositoryBase.BuscarTodos();
        }

        public virtual TEntity BuscarPeloId(int id)
        {
            var entity = _repositoryBase.BuscarPeloId(id);
            if (entity == null) throw new NotFoundException("Código não encontrado.");
            return entity;
        }

        public virtual TEntity Excluir(int id)
        {
            TEntity obj = BuscarPeloId(id);
            if (obj == null) throw new NotFoundException("Código não encontrado.");
            return Excluir(obj);
        }

        public virtual TEntity Excluir(TEntity obj)
        {
            _repositoryBase.Excluir(obj);
            _unitOfWork.Commit();
            return obj;
        }

        public virtual void Atualizar(int id, TEntity obj)
        {
            _repositoryBase.Atualizar(id, obj);
            _unitOfWork.Commit();
        }

        public virtual void Atualizar(TEntity obj)
        {
            _repositoryBase.Atualizar(obj);
            _unitOfWork.Commit();
        }

        public virtual void Dispose()
        {
            _repositoryBase.Dispose();
            _unitOfWork.Dispose();
        }
    }
}