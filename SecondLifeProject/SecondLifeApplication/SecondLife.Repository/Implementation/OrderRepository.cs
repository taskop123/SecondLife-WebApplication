using Microsoft.EntityFrameworkCore;
using SecondLife.Domain.DomainModels;
using SecondLife.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecondLife.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<Order> _entities;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
            _entities = context.Set<Order>();
        }

        public void Delete(Order entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _entities.Remove(entity);
            _context.SaveChanges();
        }

        public IEnumerable<Order> GetAll()
        {
            return _entities
                .Include(z => z.User)
                .Include(z => z.Products)
                .Include("Products.Order")
                .Include("Products.Product")
                .Include("Products.Product.Owner")
                .ToList();
        }

        public Order GetDetails(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("entity");
            }

            return this._entities
                .Include(z => z.User)
                .Include(z => z.Products)
                .Include("Products.Product")
                .Include("Products.Product.Owner")
                .SingleOrDefault(z => z.Id == id);
        }

        public IEnumerable<Order> GetUserOrders(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("entity");
            }

            return _entities
                .Include(z => z.User)
                .Include(z => z.Products)
                .Include("Products.Order")
                .Where(z => z.UserId == id);
        }

        public void Insert(Order entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _entities.Add(entity);
            _context.SaveChanges();
        }

        public void Update(Order entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _entities.Update(entity);
            _context.SaveChanges();
        }
    }
}
