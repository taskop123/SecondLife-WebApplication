using SecondLife.Domain.DomainModels;
using System;

namespace SecondLife.Domain.DomainModels
{
    public class ProductInOrder : BaseEntity
    {
        public Guid ProductId{ get; set; }
        public virtual Product Product{ get; set; }
        public Guid OrderId{ get; set; }
        public virtual Order Order{ get; set; }
        public int Quantity { get; set; }
    }
}
