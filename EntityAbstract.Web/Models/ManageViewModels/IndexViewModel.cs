using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EntityAbstract.Web.Models.ManageViewModels
{
    public class IndexViewModel
    {
        [Display(Name ="用户名")]
        public string Username { get; set; }
        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name ="邮箱")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "电话号码")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }
    }
}
