using Microsoft.AspNetCore.Identity;
using SecondLife.Domain.DomainModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecondLife.Domain.Identity
{
    public class SecondLifeApplicationUser : IdentityUser
    {
        [Required]
        public string Address { get; set; }
        public string City { get; set; }
        public string Country{ get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public string ProfilePicture { get; set; }
        public int Age { get; set; }
        public virtual ShoppingCart ShoppingCart{ get; set; }
        public virtual ICollection<Order> Orders{ get; set; }
        public virtual ICollection<Product> MyProducts{ get; set; }
    }
}
