using SecondLife.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondLife.Service.Interface
{
    public interface IOrderService
    {
        IEnumerable<Order> GetAllOrdersForUser(string id);
        IEnumerable<Order> GetAllOrders();
        Order GetOrderDetails(Guid? id);
    }
}
