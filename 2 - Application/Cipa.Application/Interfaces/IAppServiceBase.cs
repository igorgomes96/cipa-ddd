using System;
using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Application.Interfaces
{
    public interface IAppServiceBase<TEntity>: IDisposable where TEntity : class
    {
        TEntity Adicionar(TEntity obj);
        TEntity BuscarPeloId(int id);
        IEnumerable<TEntity> BuscarTodos();
        void Atualizar(TEntity obj);
        TEntity Excluir(int id);
        TEntity Excluir(TEntity obj);
    }
}