using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecondLife.Domain;
using SecondLife.Domain.Identity;
using SecondLife.Repository;
using SecondLife.Repository.Implementation;
using SecondLife.Repository.Interface;
using SecondLife.Service.Implementation;
using SecondLife.Service.Interface;
using Stripe;

namespace SecondLife.Web
{
    public class Startup
    {
        private EmailSettings emailSettings;
        public Startup(IConfiguration configuration)
        {
            emailSettings = new EmailSettings();
            Configuration = configuration;
            configuration.GetSection("EmailSettings").Bind(emailSettings);
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<SecondLifeApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();


            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });

            services.AddTransient<IProductService, SecondLife.Service.Implementation.ProductService>();
            services.AddTransient<IShoppingCartService, ShoppingCartService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IOrderService, SecondLife.Service.Implementation.OrderService>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
            services.AddScoped(typeof(IProductRepository), typeof(ProductRepository));
            services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));

            services.AddScoped<EmailSettings>(es => emailSettings);

            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));


            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
