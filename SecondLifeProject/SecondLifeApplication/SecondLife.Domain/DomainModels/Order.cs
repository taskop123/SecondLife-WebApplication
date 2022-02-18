using SecondLife.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecondLife.Domain.DomainModels
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public virtual SecondLifeApplicationUser User{ get; set; }
        public DateTime TransactionTime{ get; set; }
        public virtual ICollection<ProductInOrder> Products{ get; set; }
    }
}
