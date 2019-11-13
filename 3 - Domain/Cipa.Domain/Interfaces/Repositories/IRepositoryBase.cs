using System;
using System.Collections.Generic;

namespace Cipa.Domain.Interfaces.Repositories
{
    public interface IRepositoryBase<TEntity>: IDisposable where TEntity : class
    {
        IDisposable BeginTransaction();
        void Commit(IDisposable transaction);
        void Rollback(IDisposable transaction);
        TEntity Adicionar(TEntity obj);
        IEnumerable<TEntity> BuscarTodos();
        TEntity BuscarPeloId(int id);
        void Atualizar(TEntity obj);
        void Excluir(TEntity obj);
    }
}