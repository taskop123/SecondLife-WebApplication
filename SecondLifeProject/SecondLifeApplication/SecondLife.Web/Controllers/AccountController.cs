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
using System.Threading.Tasks;

namespace SecondLife.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<SecondLifeApplicationUser> _userManager;
        private readonly SignInManager<SecondLifeApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;

        public AccountController(IShoppingCartService shoppingCartService, IProductService productService, IWebHostEnvironment webHostEnvironment, IUserService userService, UserManager<SecondLifeApplicationUser> userManager, SignInManager<SecondLifeApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userService = userService;
            _webHostEnvironment = webHostEnvironment;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
        }

        [Authorize(Roles = "Admin, Standard User")]
        public IActionResult AddUserToRole()
        {
            AddUserToRoleDTO model = new AddUserToRoleDTO();

            List<SecondLifeApplicationUser> users = _userService.GetAllUsers().ToList();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (var user in users)
            {
                if(user.Id != userId)
                {
                    model.UserEmails.Add(user);
                }
            }

            model.Roles.Add("Standard User");
            model.Roles.Add("Admin");

            ShoppingCartDTO testModel = _shoppingCartService.GetShoppingCartInfo(User.FindFirstValue(ClaimTypes.NameIdentifier));
            double numberOfItems = testModel.ProductsInShoppingCart.Aggregate(0, (acc, z) => acc + z.Quantity);

            ViewData["NumberOfItems"] = numberOfItems;

            return View(model);
        }
        [HttpPost, Authorize(Roles = "Admin, Standard User")]
        public async Task<IActionResult> AddUserToRole(AddUserToRoleDTO model)
        {
            SecondLifeApplicationUser user = await _userManager.FindByIdAsync(model.SelectedUserId);
            List<string> roles = new List<string>
            {
                "Standard User",
                "Admin"
            };
            foreach (var i in roles)
            {
                if (await _userManager.IsInRoleAsync(user, i))
                {
                    await _userManager.RemoveFromRoleAsync(user, i);
                }
            }

            var res = await _userManager.AddToRoleAsync(user, model.SelectedRole);

            if (res.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        private string UpoloadProfilePicture(UserRegisterDTO model)
        {
            string uniqueFileName = null;

            if (model.ProfilePicture != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfilePicture.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        public IActionResult Register()
        {
            UserRegisterDTO model = new UserRegisterDTO();
            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(UserRegisterDTO model)
        {
            if (ModelState.IsValid)
            {
                var userCheck = await _userManager.FindByEmailAsync(model.Email);

                if (userCheck == null)
                {
                    var user = new SecondLifeApplicationUser
                    {
                        Email = model.Email,
                        NormalizedUserName = model.Email,
                        UserName = model.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        ShoppingCart = new ShoppingCart(),
                        Address = model.Address,
                        Age = model.Age,
                        City = model.City,
                        MyProducts = new List<Product>(),
                        Name = model.Name,
                        Country = model.Country,
                        Orders = new List<Order>(),
                        PhoneNumber = model.PhoneNumber,
                        Surname = model.Surname,
                    };
                    if(model.ProfilePicture != null)
                    {
                        user.ProfilePicture = this.UpoloadProfilePicture(model);
                    }
                    else
                    {
                        user.ProfilePicture = "profile_picture.png";
                    }

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        bool roleExists = await _roleManager.RoleExistsAsync("Standard User");

                        if (!roleExists)
                        {
                            await _roleManager.CreateAsync(new IdentityRole("Standard User"));

                        }
                        roleExists = await _roleManager.RoleExistsAsync("Admin");

                        if (!roleExists)
                        {
                            await _roleManager.CreateAsync(new IdentityRole("Admin"));
                        }

                        var res = await _userManager.AddToRoleAsync(user, "Standard User");
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            UserLoginDTO model = new UserLoginDTO();
            return View(model);
        }
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email is not confirmed!");
                    return View(model);
                }

                if(await _userManager.CheckPasswordAsync(user, model.Password) == false && user != null)
                {
                    ModelState.AddModelError("message", "Password is incorrect!");
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, true);

                if(result.Succeeded)
                {
                    //await _userManager.AddClaimAsync(user, new Claim("UserRole", "StandardUser"));
                    return RedirectToAction("Index", "Home");
                }
                else if(result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                else
                {
                    ModelState.AddModelError("message", "User is not Registered!");
                    return View(model);
                }

            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Details(string id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                ShoppingCartDTO testModel = _shoppingCartService.GetShoppingCartInfo(User.FindFirstValue(ClaimTypes.NameIdentifier));
                double numberOfItems = testModel.ProductsInShoppingCart.Aggregate(0, (acc, z) => acc + z.Quantity);

                ViewData["NumberOfItems"] = numberOfItems;
            }

            var tempUser = _userService.GetAllUsers().Where(z => z.Email == id).FirstOrDefault();
            var user = _userService.GetUserWithUserId(tempUser.Id);

            AccountDetailsDTO model = new AccountDetailsDTO()
            {
                Address = user.Address,
                Age = user.Age,
                City = user.City,
                Country = user.Country,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                ProfilePicture = user.ProfilePicture,
                Surname = user.Surname,
                UsersProducts = _productService.GetAllProductsForGivenUser(tempUser.Id)
            };

            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
