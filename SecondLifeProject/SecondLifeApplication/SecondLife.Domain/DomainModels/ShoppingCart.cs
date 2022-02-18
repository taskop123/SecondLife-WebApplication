using SecondLife.Domain.Identity;
using System;
using System.Collections.Generic;

namespace SecondLife.Domain.DomainModels
{
    public class ShoppingCart : BaseEntity
    {
        public string OwnerId { get; set; }
        public virtual SecondLifeApplicationUser Owner { get; set; }
        public virtual ICollection<ProductInShoppingCart> Products { get; set; }
    }
}
