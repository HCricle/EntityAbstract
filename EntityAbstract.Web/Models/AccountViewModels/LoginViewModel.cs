using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EntityAbstract.Web.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name ="名字")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        
        
    }
}
