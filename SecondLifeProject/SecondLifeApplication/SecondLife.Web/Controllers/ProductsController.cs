using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SecondLife.Domain.DomainModels;
using SecondLife.Domain.DTO;
using SecondLife.Domain.Identity;
using SecondLife.Repository;
using SecondLife.Service.Interface;

namespace SecondLife.Web.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly SignInManager<SecondLifeApplicationUser> _signInManager;

        public ProductsController(IProductService productService, IShoppingCartService shoppingCartService, IUserService userService, IWebHostEnvironment webHostEnvironment, SignInManager<SecondLifeApplicationUser> signInManager)
        {
            _productService = productService;
            _userService = userService;
            _webHostEnvironment = webHostEnvironment;
            _shoppingCartService = shoppingCartService;
            _signInManager = signInManager;
        }
        public IActionResult SortProductsByPriceAsc()
        {
            var products = _productService.GetAllProducts().OrderBy(z => z.Price);
            return View("Index", products);
        }
        public IActionResult SortProductsByPriceDesc()
        {
            var products = _productService.GetAllProducts().OrderByDescending(z => z.Price);
            return View("Index", products);
        }
        [HttpGet]
        public IActionResult AddProductToShoppingCart(Guid? id) 
        {
            AddProductToShoppingCartDTO model = _productService.GetShoppingCartInfo(id);

            if (_signInManager.IsSignedIn(User))
            {
                ShoppingCartDTO test = _shoppingCartService.GetShoppingCartInfo(User.FindFirstValue(ClaimTypes.NameIdentifier));
                double numberOfItems = test.ProductsInShoppingCart.Aggregate(0, (acc, z) => acc + z.Quantity);

                ViewData["NumberOfItems"] = numberOfItems;
            }

            return View(model);
        }
        [HttpPost]
        public IActionResult AddProductToShoppingCart([Bind("ProductId,Quantity")]AddProductToShoppingCartDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = _productService.AddToShoppingCart(model, userId);

            if (result)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }

            return View(model);

        }
        // GET: Products
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                ShoppingCartDTO model = _shoppingCartService.GetShoppingCartInfo(User.FindFirstValue(ClaimTypes.NameIdentifier));
                double numberOfItems = model.ProductsInShoppingCart.Aggregate(0, (acc, z) => acc + z.Quantity);

                ViewData["NumberOfItems"] = numberOfItems;
            }

            List<Product> products = _productService.GetAllProducts();

            return View(products);
        }

        // GET: Products/Details/5
        [AllowAnonymous]
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _productService.GetDetailsForProduct(id);

            if (product == null)
            {
                return NotFound();
            }
            ViewData["Product"] = product;
            ViewData["AddToShoppingCart"] = _productService.GetShoppingCartInfo(id);

            if (_signInManager.IsSignedIn(User))
            {
                ShoppingCartDTO model = _shoppingCartService.GetShoppingCartInfo(User.FindFirstValue(ClaimTypes.NameIdentifier));
                double numberOfItems = model.ProductsInShoppingCart.Aggregate(0, (acc, z) => acc + z.Quantity);

                ViewData["NumberOfItems"] = numberOfItems;
            }

            return View();
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            if (_signInManager.IsSignedIn(User))
            {
                ShoppingCartDTO model = _shoppingCartService.GetShoppingCartInfo(User.FindFirstValue(ClaimTypes.NameIdentifier));
                double numberOfItems = model.ProductsInShoppingCart.Aggregate(0, (acc, z) => acc + z.Quantity);

                ViewData["NumberOfItems"] = numberOfItems;
            }

            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductCreationDTO model)
        {
            if (ModelState.IsValid)
            {

                string uniqueFileName = UploadedFile(model);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _userService.GetUserWithUserId(userId);

                Product product = new Product()
                {
                    Id = Guid.NewGuid(),
                    Price = model.Price,
                    Category = model.Category,
                    Gender = model.Gender,
                    Manufacturer = model.Manufacturer,
                    Owner = user,
                    OwnerId = userId,
                    ProductDescription = model.ProductDescription,
                    ProductImage = uniqueFileName,
                    ProductionYear = model.ProductionYear,
                    ProductName = model.ProductName,
                    Quantity = model.Quantity,
                    Size = model.Size
                };

                _productService.CreateNewProduct(product);
                _userService.AddNewProductToUser(userId, product);

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        private string UploadedFile(ProductCreationDTO model)
        {
            string uniqueFileName = null;

            if (model.ProductImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProductImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProductImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        private string UploadedFileForEdit(ProductEditDTO model)
        {
            string uniqueFileName = null;

            if (model.ProductImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProductImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProductImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        // GET: Products/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                ShoppingCartDTO model_ = _shoppingCartService.GetShoppingCartInfo(User.FindFirstValue(ClaimTypes.NameIdentifier));
                double numberOfItems = model_.ProductsInShoppingCart.Aggregate(0, (acc, z) => acc + z.Quantity);

                ViewData["NumberOfItems"] = numberOfItems;
            }

            if (id == null)
            {
                return NotFound();
            }

            var product =_productService.GetDetailsForProduct(id);
            ProductEditDTO model = new ProductEditDTO()
            {
                Category = product.Category,
                Gender = product.Gender,
                Manufacturer = product.Manufacturer,
                Price = product.Price,
                ProductDescription = product.ProductDescription,
                ProductionYear = product.ProductionYear,
                ProductName = product.ProductName,
                Quantity = product.Quantity,
                Id = product.Id,
                Size = product.Size
            };

            if (product == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,Price,Quantity,ProductName,ProductDescription,ProductImage,ProductionYear,Manufacturer,Gender,Category,Size")] ProductEditDTO product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var productToUpdate = _productService.GetDetailsForProduct(product.Id);
                    productToUpdate.Manufacturer = product.Manufacturer;
                    productToUpdate.Price = product.Price;
                    productToUpdate.ProductDescription = product.ProductDescription;
                    if(product.ProductImage != null)
                    {
                        productToUpdate.ProductImage = this.UploadedFileForEdit(product);
                    }
                    productToUpdate.ProductionYear = product.ProductionYear;
                    productToUpdate.Quantity = product.Quantity;
                    productToUpdate.ProductName = product.ProductName;
                    productToUpdate.Gender = product.Gender;
                    productToUpdate.Category = product.Category;
                    productToUpdate.Size = product.Size;

                    _productService.UpdateExistingProduct(productToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _productService.GetDetailsForProduct(id);
            var result = _productService.DeleteProduct(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _userService.RemoveProductFromUser(userId, product);
            if(result)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _productService.DeleteProduct(id);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid id)
        {
            return _productService.GetDetailsForProduct(id) != null;
        }
    }
}
