using SecondLife.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondLife.Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<SecondLifeApplicationUser> GetAll();
        SecondLifeApplicationUser Get(string id);
        void Insert(SecondLifeApplicationUser entity);
        void Update(SecondLifeApplicationUser entity);
        void Delete(SecondLifeApplicationUser entity);
    }
}
