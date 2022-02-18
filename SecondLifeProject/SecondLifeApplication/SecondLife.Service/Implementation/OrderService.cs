using SecondLife.Domain.DomainModels;
using SecondLife.Repository.Interface;
using SecondLife.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecondLife.Service.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public IEnumerable<Order> GetAllOrders()
        {
            return _orderRepository.GetAll().AsEnumerable();
        }

        public IEnumerable<Order> GetAllOrdersForUser(string id)
        {
            return _orderRepository.GetUserOrders(id).AsEnumerable();
        }

        public Order GetOrderDetails(Guid? id)
        {
            return _orderRepository.GetDetails(id);
        }
    }
}
