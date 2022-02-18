using SecondLife.Domain.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecondLife.Domain.DTO
{
    public class AddUserToRoleDTO
    {
        [Display(Name = "User Emails")]
        public List<SecondLifeApplicationUser> UserEmails { get; set; }
        [Display(Name = "Active Roles")]
        public List<string> Roles { get; set; }
        [Display(Name = "User Email")]
        public string SelectedUserId { get; set; }
        [Display(Name = "Role")]
        public string SelectedRole { get; set; }
        public AddUserToRoleDTO()
        {
            this.UserEmails = new List<SecondLifeApplicationUser>();
            this.Roles = new List<string>();
        }
    }
}
