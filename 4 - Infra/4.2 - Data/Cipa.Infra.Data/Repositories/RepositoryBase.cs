using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Cipa.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Cipa.Domain.Exceptions;
using Cipa.Infra.Data.Context;
using System.Linq;

namespace Cipa.Infra.Data.Repositories
{
    public class RepositoryBase<TEntity> : IDisposable, IRepositoryBase<TEntity> where TEntity : class
    {
        protected readonly CipaContext _db;
        public RepositoryBase(CipaContext db)
        {
            _db = db;
        }

        protected DbSet<TEntity> DbSet => _db.Set<TEntity>();

        public virtual TEntity Adicionar(TEntity obj) => DbSet.Add(obj).Entity;

        public virtual IEnumerable<TEntity> BuscarTodos() => DbSet;

        public virtual TEntity BuscarPeloId(int id) => DbSet.Find(id);

        public virtual void Excluir(TEntity obj) => DbSet.Remove(obj);

        public virtual void Atualizar(TEntity obj) => _db.Entry(obj).State = EntityState.Modified;

        public virtual void Dispose() => _db.Dispose();

    }
}