using System;
using System.Collections.Generic;
using Cipa.Application.Interfaces;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Interfaces.Repositories;

namespace Cipa.Application {
    public class AppServiceBase<TEntity> : IDisposable, IAppServiceBase<TEntity> where TEntity : class
    {
        private readonly IRepositoryBase<TEntity> _repositoryBase;
        private readonly IUnitOfWork _unitOfWork;

        public AppServiceBase(IUnitOfWork unitOfWork, IRepositoryBase<TEntity> repositoryBase) {
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
            return _repositoryBase.BuscarPeloId(id);
        }

        public virtual TEntity Excluir(int id)
        {
            TEntity obj = BuscarPeloId(id);
            if (obj == null) throw new NotFoundException("Código não encontrado.");
            _repositoryBase.Excluir(obj);
            _unitOfWork.Commit();
            return obj;
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