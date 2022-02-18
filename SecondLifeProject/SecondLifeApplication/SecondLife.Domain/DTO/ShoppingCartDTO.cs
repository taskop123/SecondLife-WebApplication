using SecondLife.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondLife.Domain.DTO
{
    public class ShoppingCartDTO
    {
        public List<ProductInShoppingCart> ProductsInShoppingCart { get; set; }
        public double TotalPrice { get; set; }
    }
}
