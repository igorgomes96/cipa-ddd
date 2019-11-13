using System;
using System.Collections.Generic;

namespace Cipa.Domain.Interfaces.Services
{
    public interface IServiceBase<TEntity>: IDisposable where TEntity : class
    {
        TEntity Adicionar(TEntity obj);
        TEntity BuscarPeloId(int id);
        IEnumerable<TEntity> BuscarTodos();
        void Atualizar(TEntity obj);
        void Excluir(TEntity obj);
    }
}