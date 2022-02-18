using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace SecondLife.Domain.DTO
{
    public class UserRegisterDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public int Age { get; set; }
        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }
        public string Address { get; set; }
        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "This field is Required!")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
