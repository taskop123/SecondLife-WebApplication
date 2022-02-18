using SecondLife.Domain.DomainModels;
using SecondLife.Domain.DTO;
using SecondLife.Domain.Identity;
using SecondLife.Repository.Interface;
using SecondLife.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecondLife.Service.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<ProductInShoppingCart> _productInShoppingCartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IRepository<ProductInOrder> _productInOrderRepository;
        private readonly IRepository<EmailMessage> _emailRepository;

        private readonly IEmailService _emailService;

        public ShoppingCartService(IRepository<EmailMessage> emailRepository, IEmailService emailService, IRepository<ProductInOrder> productInOrderRepository, IOrderRepository orderRepository, IProductRepository productRepository, IRepository<ShoppingCart> shoppingCartRepository, IUserRepository userRepository, IRepository<ProductInShoppingCart> productInShoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _userRepository = userRepository;
            _productInShoppingCartRepository = productInShoppingCartRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _productInOrderRepository = productInOrderRepository;
            _emailService = emailService;
            _emailRepository = emailRepository;
        }

        public void AddOneMoreQuantityToProduct(Guid? shoppingCartId, Guid? productId, int quantity)
        {
            var product = _productRepository.Get(productId);
            if(product.Quantity - quantity < 0)
            {
                return;
            }

            var productInShoppingCart = _productInShoppingCartRepository.GetAll().Where(z => z.ShoppingCartId == shoppingCartId && z.ProductId == productId).FirstOrDefault();

            if(productInShoppingCart.Quantity + quantity <= 0)
            {
                product.Quantity += 1;
                _productInShoppingCartRepository.Delete(productInShoppingCart);
                _productRepository.Update(product);

                return;
            }

            productInShoppingCart.Quantity += quantity;
            product.Quantity -= quantity;

            _productRepository.Update(product);
            _productInShoppingCartRepository.Update(productInShoppingCart);
        }

        public bool DeleteProductFromShoppingCart(string userId, Guid id)
        {
            var user = _userRepository.Get(userId);
            var shoppingCartId = user.ShoppingCart.Id;
            if(user == null)
            {
                return false;
            }

            var productInShoppingCart = _productInShoppingCartRepository.GetAll().ToList();
            foreach(var item in productInShoppingCart)
            {
                if(item.ProductId == id && item.ShoppingCartId == shoppingCartId)
                {
                    var productInSC = _productInShoppingCartRepository.Get(item.Id);
                    var product = _productRepository.Get(productInSC.ProductId);
                    product.Quantity += productInSC.Quantity;
                    _productInShoppingCartRepository.Delete(productInSC);
                    _productRepository.Update(product);
                }
            }

            return true;
        }

        public ShoppingCartDTO GetShoppingCartInfo(string userId)
        {
            var user = _userRepository.Get(userId);
            if (user == null)
            {
                return null;
            }

            var userCart = user.ShoppingCart;

            var productPrice = userCart.Products.Select(z => new
            {
                ProductPrice = z.Product.Price,
                Quantity = z.Quantity
            }).ToList();

            float totalPrice = 0;

            ShoppingCartDTO model = new ShoppingCartDTO();

            foreach (var item in productPrice)
            {
                totalPrice += (float)item.Quantity * item.ProductPrice;
            }

            model.TotalPrice = totalPrice;
            model.ProductsInShoppingCart = userCart.Products.ToList();

            return model;
        }

        public bool OrderNow(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            SecondLifeApplicationUser user = _userRepository.Get(userId);
            ShoppingCart shoppingCart = user.ShoppingCart;

            Order order = new Order();
            order.Id = Guid.NewGuid();
            order.User = user;
            order.UserId = userId;
            order.TransactionTime = DateTime.Now;

            _orderRepository.Insert(order);

            ICollection<ProductInOrder> products = new LinkedList<ProductInOrder>();
            ICollection<string> owners = new SortedSet<string>();
            foreach (var item in shoppingCart.Products)
            {
                ProductInOrder productInOrder = new ProductInOrder
                {
                    Id = Guid.NewGuid(),
                    Order = order,
                    OrderId = order.Id,
                    Product = item.Product,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                products.Add(productInOrder);
                _productInOrderRepository.Insert(productInOrder);

                owners.Add(item.Product.OwnerId);
            }

            order.Products = products;
            _orderRepository.Update(order);

            user.ShoppingCart.Products.Clear();
            _userRepository.Update(user);

            EmailMessage emailMessage = new EmailMessage();
            emailMessage.MailTo = user.Email;
            emailMessage.Subject = "Successfully Created Order!";
            emailMessage.Status = false;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Your order is completed. Thank you for using SecondLife application and finding home for used clothes!\nYour order will be shipped as soon as possible.");
            sb.AppendLine("The Order contains: ");
            var totalPrice = 10.0;

            for (int i = 0; i < products.ToList().Count; i++)
            {
                var product = products.ToList()[i];
                sb.AppendLine((i + 1).ToString() + ". Product Name: " + product.Product.ProductName + " with price of: $" + product.Product.Price + " and quantity: " + product.Quantity);
                totalPrice += product.Quantity * product.Product.Price;
            }

            sb.AppendLine("Total : $" + totalPrice.ToString());
            sb.AppendLine();
            sb.AppendLine("Congratulations on your Order!\n");
            sb.AppendLine("Yours Sincerely: \nSecondLife Team!");
            emailMessage.Content = sb.ToString();

            _emailRepository.Insert(emailMessage);
            _emailService.SendEmailAsync(emailMessage);

            foreach(var ownerId in owners)
            {
                var person = _userRepository.Get(ownerId);
                EmailMessage message = new EmailMessage();
                message.MailTo = person.Email;
                message.Subject = "Successfully sold item!";
                message.Status = false;
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendLine("Your item was sold to: " + user.Name + " " + user.Surname + ", with email address: " + user.Email);
                stringBuilder.AppendLine("The Order contains: ");
                totalPrice = 10.0;

                for (int i = 0; i < products.ToList().Count; i++)
                {
                    
                    var product = products.ToList()[i];
                    
                    if(product.Product.OwnerId == ownerId)
                    {
                        stringBuilder.AppendLine((i + 1).ToString() + ". Product Name: " + product.Product.ProductName + ", Quantity: " + product.Quantity + " and with price of: " + product.Product.Price);
                        totalPrice += product.Quantity * product.Product.Price;
                    }
                }

                stringBuilder.AppendLine("Total: $" + totalPrice.ToString());
                stringBuilder.AppendLine("You need to send this order to the following address: ");
                stringBuilder.AppendLine("Address: " + user.Address + ", Country: " + user.Country + " and City: " + user.City + ".");
                stringBuilder.AppendLine("For contact with: " + user.Name + " " + user.Surname + ", contact him via phone number: " + user.PhoneNumber + ".");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Congratulations on your sale!\n");
                stringBuilder.AppendLine("Yours Sincerely: \nSecondLife Team!");
                message.Content = stringBuilder.ToString();

                _emailRepository.Insert(message);
                _emailService.SendEmailAsync(message);
            }


            return true;
        }
    }
}
