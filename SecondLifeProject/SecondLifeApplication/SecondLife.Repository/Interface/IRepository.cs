using SecondLife.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondLife.Repository.Interface
{
    public interface IRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        T Get(Guid? id);
        int Insert(T entity);
        int Update(T entity);
        int Delete(T entity);
    }
}
