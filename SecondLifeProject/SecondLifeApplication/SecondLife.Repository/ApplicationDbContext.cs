using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecondLife.Domain.Identity;
using SecondLife.Domain.DomainModels;
using System;

namespace SecondLife.Repository
{
    public class ApplicationDbContext : IdentityDbContext<SecondLifeApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<ProductInOrder> ProductsInOrders { get; set; }
        public virtual DbSet<ProductInShoppingCart> ProductsInShoppingCarts { get; set; }
        public virtual DbSet<EmailMessage> EmailMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
                .Property(p => p.Category)
                .HasConversion(
                    v => v.ToString(),
                    v => (Category)Enum.Parse(typeof(Category), v)
                 );

            builder.Entity<Product>()
                .Property(p => p.Gender)
                .HasConversion(
                    v => v.ToString(),
                    v => (Gender)Enum.Parse(typeof(Gender), v)
                 );
            builder.Entity<Product>()
               .Property(p => p.Size)
               .HasConversion(
                   v => v.ToString(),
                   v => (Size)Enum.Parse(typeof(Size), v)
                );

            builder.Entity<Product>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<ShoppingCart>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<ProductInShoppingCart>()
                .HasOne(z => z.Product)
                .WithMany(z => z.ShoppingCarts)
                .HasForeignKey(z => z.ProductId);

            builder.Entity<ProductInShoppingCart>()
                .HasOne(z => z.ShoppingCart)
                .WithMany(z => z.Products)
                .HasForeignKey(z => z.ShoppingCartId);

            builder.Entity<ProductInOrder>()
                .HasOne(z => z.Product)
                .WithMany(z => z.Orders)
                .HasForeignKey(z => z.ProductId);

            builder.Entity<ProductInOrder>()
                .HasOne(z => z.Order)
                .WithMany(z => z.Products)
                .HasForeignKey(z => z.OrderId);

            builder.Entity<ShoppingCart>()
                .HasOne<SecondLifeApplicationUser>(z => z.Owner)
                .WithOne(z => z.ShoppingCart)
                .HasForeignKey<ShoppingCart>(z => z.OwnerId);

            builder.Entity<Order>()
                .HasOne<SecondLifeApplicationUser>(z => z.User)
                .WithMany(z => z.Orders)
                .HasForeignKey(z => z.UserId);

            builder.Entity<Product>()
                .HasOne<SecondLifeApplicationUser>(z => z.Owner)
                .WithMany(z => z.MyProducts)
                .HasForeignKey(z => z.OwnerId);
        }

    }
}
