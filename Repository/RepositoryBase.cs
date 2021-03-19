using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext RepositoryContext;
        public RepositoryBase(RepositoryContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public void Create(T entity)
        {
            RepositoryContext.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            RepositoryContext.Set<T>().Remove(entity);
        }

        public IQueryable<T> FindAll(bool trackChanges)
        {
            return !trackChanges ? RepositoryContext.Set<T>().AsNoTracking() : RepositoryContext.Set<T>();
            //om query sneller resultaat terug te laten geven kan je de automatische Tracking van EF Core afzetten.
            //EF Core gebruikt Tracking om wijzigingen, toevoegingen en verwijderingen van entities uit DbContext bij te houden
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
        {
           return !trackChanges ? RepositoryContext.Set<T>().Where(expression).AsNoTracking() : RepositoryContext.Set<T>() .Where(expression);
           //expression kan bv zijn: e => e.Naam=="Jos" of e=> e.Id == 1 , bv bij Company: e=> e.Country=="USA"...

        }

        public void Update(T entity)
        {
            RepositoryContext.Set<T>().Update(entity);
        }
    }
}
