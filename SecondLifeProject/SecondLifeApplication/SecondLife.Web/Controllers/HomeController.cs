using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecondLife.Domain;
using SecondLife.Domain.DomainModels;
using SecondLife.Domain.DTO;
using SecondLife.Domain.Identity;
using SecondLife.Service.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SecondLife.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly SignInManager<SecondLifeApplicationUser> _signInManager;
        private readonly IProductService _productService;

        public HomeController(IProductService productService, ILogger<HomeController> logger, IShoppingCartService shoppingCartService, SignInManager<SecondLifeApplicationUser> signInManager)
        {
            _shoppingCartService = shoppingCartService;
            _logger = logger;
            _signInManager = signInManager;
            _productService = productService;
        }

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                ShoppingCartDTO model = _shoppingCartService.GetShoppingCartInfo(User.FindFirstValue(ClaimTypes.NameIdentifier));
                double numberOfItems = model.ProductsInShoppingCart.Aggregate(0, (acc, z) => acc + z.Quantity);

                ViewData["NumberOfItems"] = numberOfItems;
            }
            List<Product> products = _productService.GetAllProducts().ToList().Take(3).ToList();
            
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
