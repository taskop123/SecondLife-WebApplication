using SecondLife.Domain.DomainModels;
using SecondLife.Domain.Identity;
using SecondLife.Repository.Interface;
using SecondLife.Service.Interface;
using System;
using System.Collections.Generic;

namespace SecondLife.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool AddNewProductToUser(string userId, Product p)
        {
            var user = _userRepository.Get(userId);
            user.MyProducts.Add(p);
            _userRepository.Update(user);
            return true;
        }

        public IEnumerable<SecondLifeApplicationUser> GetAllUsers()
        {
            return _userRepository.GetAll();
        }

        public SecondLifeApplicationUser GetUserWithUserId(string? Id)
        {
            return _userRepository.Get(Id);
        }

        public void RemoveProductFromUser(string userId, Product p)
        {
            var user = _userRepository.Get(userId);
            user.MyProducts.Remove(p);
            _userRepository.Update(user);
        }
    }
}
