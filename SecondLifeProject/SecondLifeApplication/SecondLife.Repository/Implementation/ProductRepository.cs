using Microsoft.EntityFrameworkCore;
using SecondLife.Domain.DomainModels;
using SecondLife.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecondLife.Repository.Implementation
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<Product> _entities;
        string errorMessage = string.Empty;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
            _entities = context.Set<Product>();
        }
        public int Delete(Product entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _entities.Remove(entity);
            return _context.SaveChanges();
        }

        public Product Get(Guid? id)
        {
            return _entities
                .Include(z => z.Owner)
                .SingleOrDefault(z => z.Id == id);
        }

        public IEnumerable<Product> GetAll()
        {
            return _entities
                .Include(z => z.Owner)
                .AsEnumerable();
        }

        public int Insert(Product entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _entities.Add(entity);
            return _context.SaveChanges();
        }

        public int Update(Product entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _entities.Update(entity);
            return _context.SaveChanges();
        }
    }
}
