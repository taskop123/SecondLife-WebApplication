using SecondLife.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecondLife.Domain.DomainModels
{

    public enum Category
    {
        TopClothing,
        BottomClothing,
        Shoes,
        Jackets,
        Accessories,
    }
    public enum Gender
    {
        Male,
        Female,
        UniSex
    }
    public enum Size
    {
        XS,
        S,
        M,
        L,
        XL,
        XXL,
        XXXL
    }
    public class Product : BaseEntity
    {
        [Required]
        public float Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Display(Name = "Product Description")]

        public string ProductDescription { get; set; }
        [Required]
        [Display(Name = "Product Image")]

        public string ProductImage { get; set; }
        [Display(Name = "Production Year")]

        public int ProductionYear { get; set; }
        public string Manufacturer { get; set; }
        public Category Category{ get; set; }
        public Gender Gender{ get; set; }
        public Size Size { get; set; }

        public string OwnerId { get; set; }
        public virtual SecondLifeApplicationUser Owner{ get; set; }
        public virtual ICollection<ProductInShoppingCart> ShoppingCarts { get; set; }
        public virtual ICollection<ProductInOrder> Orders{ get; set; }
    }
}
