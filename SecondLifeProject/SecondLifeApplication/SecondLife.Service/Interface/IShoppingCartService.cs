using SecondLife.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondLife.Service.Interface
{
    public interface IShoppingCartService
    {
        ShoppingCartDTO GetShoppingCartInfo(string userId);
        bool DeleteProductFromShoppingCart(string userId, Guid id);
        bool OrderNow(string userId);
        void AddOneMoreQuantityToProduct(Guid? shoppingCartId, Guid? productId, int quantity);
    }
}
