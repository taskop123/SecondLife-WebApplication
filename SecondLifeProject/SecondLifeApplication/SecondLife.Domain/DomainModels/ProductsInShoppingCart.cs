using System;

namespace SecondLife.Domain.DomainModels
{
    public class ProductInShoppingCart : BaseEntity
    {
        public Guid ProductId{ get; set; }
        public virtual Product Product{ get; set; }
        public Guid ShoppingCartId{ get; set; }
        public virtual ShoppingCart ShoppingCart{ get; set; }
        public int Quantity { get; set; }
    }
}
