using GemBox.Document;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecondLife.Domain.DomainModels;
using SecondLife.Domain.DTO;
using SecondLife.Domain.Identity;
using SecondLife.Service.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SecondLife.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly SignInManager<SecondLifeApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _appEnvironment;

        public OrdersController(IWebHostEnvironment appEnvironment, IOrderService orderService, IShoppingCartService shoppingCartService, SignInManager<SecondLifeApplicationUser> signInManager)
        {
            _orderService = orderService;
            _signInManager = signInManager;
            _shoppingCartService = shoppingCartService;
            _appEnvironment = appEnvironment;
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                ShoppingCartDTO model = _shoppingCartService.GetShoppingCartInfo(User.FindFirstValue(ClaimTypes.NameIdentifier));
                double numberOfItems = model.ProductsInShoppingCart.Aggregate(0, (acc, z) => acc + z.Quantity);

                ViewData["NumberOfItems"] = numberOfItems;
            }

            List<Order> orders = _orderService.GetAllOrders().ToList();

            return View(orders);
        }
        public IActionResult CreateInvoice(Guid? id)
        {
            Order order = _orderService.GetOrderDetails(id);

            var templatePath = Path.Combine(_appEnvironment.WebRootPath, "Invoice.docx");
            var document = DocumentModel.Load(templatePath);

            document.Content.Replace("{{OrderNumber}}", order.Id.ToString());
            document.Content.Replace("{{UserName}}", order.User.Email);
            

            StringBuilder sb = new StringBuilder();
            HashSet<SecondLifeApplicationUser> owners = new HashSet<SecondLifeApplicationUser>();

            var totalPrice = 10.0;

            foreach (var product in order.Products)
            {
                sb.Append("Product Name: ").Append(product.Product.ProductName).Append(", with price: $")
                    .Append(product.Product.Price).Append(" and quantity: ").Append(product.Quantity).Append("\n");

                totalPrice += product.Quantity * product.Product.Price;

                owners.Add(product.Product.Owner);
            }

            document.Content.Replace("{{ProductList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", "$" + totalPrice.ToString());

            sb = new StringBuilder();

            foreach(var owner in owners)
            {
                sb.Append("Owner Name: ").Append(owner.Name + " " + owner.Surname).Append(", with address: ").Append(owner.Address).Append("\n");
            }
            document.Content.Replace("{{OwnerDetails}}", sb.ToString());

            document.Content.Replace("{{OrderTransactionTime}}", order.TransactionTime.ToString());

            MemoryStream ms = new MemoryStream();
            document.Save(ms, new PdfSaveOptions());

            return File(ms.ToArray(), new PdfSaveOptions().ContentType, "ExportInvoice.pdf");
        }
    }
}
