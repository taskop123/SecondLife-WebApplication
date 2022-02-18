using Microsoft.AspNetCore.Http;
using SecondLife.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecondLife.Domain.DTO
{
    public class ProductCreationDTO
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

        public IFormFile ProductImage { get; set; }
        [Display(Name = "Production Year")]

        public int ProductionYear { get; set; }
        public string Manufacturer { get; set; }
        public Category Category { get; set; }
        public Gender Gender { get; set; }
        public Size Size { get; set; }
    }
}
