using SecondLife.Domain.DomainModels;
using SecondLife.Domain.DTO;
using SecondLife.Repository.Interface;
using SecondLife.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecondLife.Service.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<ProductInShoppingCart> _productInShoppingCart;

        public ProductService(IRepository<ProductInShoppingCart> productInShoppingCart, IProductRepository productRepository, IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _productInShoppingCart = productInShoppingCart;
        }

        public bool AddToShoppingCart(AddProductToShoppingCartDTO item, string userId)
        {
            var user = _userRepository.Get(userId);
            var shoppingCart = user.ShoppingCart;

            if(item.ProductId != null && shoppingCart != null)
            {
                Product product = _productRepository.Get(item.ProductId);

                IList<ProductInShoppingCart> productInShoppingCarts = _productInShoppingCart.GetAll().ToList();

                foreach(var i in productInShoppingCarts)
                {
                    if(i.ProductId.Equals(item.ProductId) && i.ShoppingCartId.Equals(shoppingCart.Id))
                    {
                        var productInShoppingCart = _productInShoppingCart.Get(i.Id);
                        productInShoppingCart.Quantity += item.Quantity;
                        _productInShoppingCart.Update(productInShoppingCart);

                        product.Quantity -= item.Quantity;
                        _productRepository.Update(product);

                        return true;
                    }
                }
                if(product != null)
                {
                    ProductInShoppingCart model = new ProductInShoppingCart()
                    {
                        ShoppingCart = shoppingCart,
                        ShoppingCartId = shoppingCart.Id,
                        Product = product,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    };

                    _productInShoppingCart.Insert(model);

                    product.Quantity -= item.Quantity;
                    _productRepository.Update(product);

                    return true;
                }
                return false;
            }
            return false;
        }

        public bool CreateNewProduct(Product t)
        {
            var result = _productRepository.Insert(t);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteProduct(Guid? id)
        {
            var product = _productRepository.Get(id);
            if(product == null)
            {
                return false;
            }
            var result = _productRepository.Delete(product);
            if(result > 0)
            {
                
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Product> GetAllProducts()
        {
            return _productRepository.GetAll().ToList();
        }

        public List<Product> GetAllProductsForGivenUser(string userId)
        {
            return _productRepository.GetAll()
                .Where(z => z.OwnerId == userId)
                .ToList();
        }

        public Product GetDetailsForProduct(Guid? id)
        {
            return _productRepository.Get(id);
        }

        public List<Product> GetProductsByCategory(string category)
        {
            return _productRepository.GetAll()
                .Where(z => z.Category.ToString() == category)
                .ToList();
        }

        public List<Product> GetProductsByGender(string gender)
        {
            return _productRepository.GetAll()
                .Where(z => z.Gender.ToString() == gender)
                .ToList();
        }

        public AddProductToShoppingCartDTO GetShoppingCartInfo(Guid? id)
        {
            Product product = _productRepository.Get(id);
            AddProductToShoppingCartDTO model = new AddProductToShoppingCartDTO()
            {
                Product = product,
                ProductId = product.Id,
                Quantity = 1,
                MaxQuantity = product.Quantity
            };
            return model;
           
        }

        public bool UpdateExistingProduct(Product t)
        {
            var result = _productRepository.Update(t);
            if(result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
