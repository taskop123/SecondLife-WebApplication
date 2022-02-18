using SecondLife.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondLife.Domain.DTO
{
    public class AccountDetailsDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string ProfilePicture { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<Product> UsersProducts { get; set; }
    }
}
