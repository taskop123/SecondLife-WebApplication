using SecondLife.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondLife.Repository.Interface
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        Product Get(Guid? id);
        int Insert(Product entity);
        int Update(Product entity);
        int Delete(Product entity);
    }
}
