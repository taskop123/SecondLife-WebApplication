using Microsoft.EntityFrameworkCore;
using SecondLife.Domain.DomainModels;
using SecondLife.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SecondLife.Repository.Implementation
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext context;
        private DbSet<T> entities;
        string errorMessage = string.Empty;

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }
        public int Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            return context.SaveChanges();
        }

        public T Get(Guid? id)
        {
            return entities.SingleOrDefault(z => z.Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            return entities.AsEnumerable();
        }

        public int Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            return context.SaveChanges();
        }

        public int Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            return context.SaveChanges();
        }
    }
}
