using Microsoft.EntityFrameworkCore;
using SecondLife.Domain.Identity;
using SecondLife.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SecondLife.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {

        private readonly ApplicationDbContext _context;
        private DbSet<SecondLifeApplicationUser> _entities;
        string errormessage = string.Empty;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
            _entities = context.Set<SecondLifeApplicationUser>();
        }

        public void Delete(SecondLifeApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _entities.Remove(entity);
            _context.SaveChanges();
        }

        public SecondLifeApplicationUser Get(string id)
        {
            return _entities
                .Include(z => z.ShoppingCart)
                .Include(z => z.MyProducts)
                .Include("ShoppingCart.Products")
                .Include("ShoppingCart.Products.Product")
                .SingleOrDefault(z => z.Id == id);
        }

        public IEnumerable<SecondLifeApplicationUser> GetAll()
        {
            return _entities.AsEnumerable();
        }

        public void Insert(SecondLifeApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _entities.Add(entity);
            _context.SaveChanges();
        }

        public void Update(SecondLifeApplicationUser entity)
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
