using SecondLife.Domain.DomainModels;
using SecondLife.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondLife.Service.Interface
{
    public interface IProductService
    {
        List<Product> GetAllProducts();
        Product GetDetailsForProduct(Guid? id);
        bool CreateNewProduct(Product t);
        bool UpdateExistingProduct(Product t);
        AddProductToShoppingCartDTO GetShoppingCartInfo(Guid? id);
        bool DeleteProduct(Guid? id);
        bool AddToShoppingCart(AddProductToShoppingCartDTO item, string userId);
        List<Product> GetProductsByCategory(string category);
        List<Product> GetProductsByGender(string gender);
        List<Product> GetAllProductsForGivenUser(string userId);
    }
}
