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

        protected DbSet<TEntity> DbSet
        {
            get
            {
                return _db.Set<TEntity>();
            }
        }

        public virtual TEntity Adicionar(TEntity obj)
        {
            TEntity newObj = DbSet.Add(obj).Entity;
            _db.SaveChanges();
            return newObj;
        }

        public virtual IEnumerable<TEntity> BuscarTodos()
        {
            return DbSet;
        }

        public virtual TEntity BuscarPeloId(int id)
        {
            return DbSet.Find(id);
        }

        public virtual void Excluir(TEntity obj)
        {
            DbSet.Remove(obj);
            _db.SaveChanges();
        }

        public virtual void Atualizar(TEntity obj)
        {
            _db.Entry(obj).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public virtual void Dispose()
        {
            _db.Dispose();
        }

        public IDisposable BeginTransaction()
        {
            return _db.Database.BeginTransaction();
        }

        public void Commit(IDisposable transaction)
        {
            if (transaction == null)
            {
                throw new CustomException("Não há nenhuma transação aberta.");
            }
            ((IDbContextTransaction)transaction).Commit();
        }

        public void Rollback(IDisposable transaction)
        {
            if (transaction == null)
            {
                throw new CustomException("Não há nenhuma transação aberta.");
            }
            ((IDbContextTransaction)transaction).Rollback();
        }
    }
}