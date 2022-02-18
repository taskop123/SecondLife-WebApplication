using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondLife.Domain.DTO;
using SecondLife.Service.Interface;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SecondLife.Web.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserService _userService;
        public ShoppingCartController(IShoppingCartService shoppingCartService, IUserService userService)
        {
            _shoppingCartService = shoppingCartService;
            _userService = userService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userService.GetUserWithUserId(userId);

            ViewData["UserImage"] = user.ProfilePicture;
            ViewData["UserName"] = user.Name;
            ViewData["UserLastName"] = user.Surname;
            ViewData["UserAddress"] = user.Address;

            ShoppingCartDTO model = _shoppingCartService.GetShoppingCartInfo(userId);
            double numberOfItems = model.ProductsInShoppingCart.Aggregate(0, (acc, z) => acc + z.Quantity);
            ViewData["NumberOfItems"] = numberOfItems;

            return View(model);
        }
        [HttpGet]
        public IActionResult AddOneMoreQuantity(Guid shoppingCartId, Guid productId)
        {
            _shoppingCartService.AddOneMoreQuantityToProduct(shoppingCartId, productId, 1);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult RemoveOneQuantity(Guid shoppingCartId, Guid productId)
        {
            _shoppingCartService.AddOneMoreQuantityToProduct(shoppingCartId, productId, -1);
            return RedirectToAction("Index");
        }

        public IActionResult DeleteTicketFromShoppingCart(Guid ticketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _shoppingCartService.DeleteProductFromShoppingCart(userId, ticketId);

            return RedirectToAction("Index");
        }
        public IActionResult PayOrder(string stripeEmail, string stripeToken)
        {
            var customerService = new CustomerService();
            var chargeService = new ChargeService();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = _shoppingCartService.GetShoppingCartInfo(userId);

            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var result = chargeService.Create(new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(order.TotalPrice) * 100,
                Description = "Second Life App Payment",
                Currency = "usd",
                Customer = customer.Id
            });

            if (result.Status == "succeeded")
            {
                var res = _shoppingCartService.OrderNow(userId);
                if (res)
                {
                    TempData["ResponseToOrder"] = "Successfull|Your payment is successfull! For more details check out your email!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ResponseToOrder"] = "Warning|Your payment is successfull! But something went wrong while completing your order!";

                    return RedirectToAction("Index");
                }
            }
            TempData["ResponseToOrder"] = "Error|Oops, something went wrong! Please try again later!";

            return RedirectToAction("Index");
        }

    }
}
