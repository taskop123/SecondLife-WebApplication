using SecondLife.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondLife.Domain.DTO
{
    public class AddProductToShoppingCartDTO
    {
        public Guid ProductId { get; set; }
        public Product Product{ get; set; }
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }
    }
}
