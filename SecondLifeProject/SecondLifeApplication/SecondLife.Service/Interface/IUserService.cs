using SecondLife.Domain.DomainModels;
using SecondLife.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondLife.Service.Interface
{
    public interface IUserService
    {
        IEnumerable<SecondLifeApplicationUser> GetAllUsers();
        SecondLifeApplicationUser GetUserWithUserId(string? Id);
        bool AddNewProductToUser(string userId, Product p);
        void RemoveProductFromUser(string userId, Product p);
    }
}
