using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BKBBoard.Models
{
    public class LoginModels
    {
        [Required(ErrorMessage = "Please enter the User Name")]
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        [Display(Name = "User Name")]
        public string userName { get; set; }

        [Required(ErrorMessage = "Please enter the Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Branch Name")]
        public string BranchCode { get; set; }

        [Display(Name = "User Status")]
        public string UserStatus { get; set; }

        public string role_priority { get; set; }

        public int? UserRoleId { get; set; }
        public string RoleName { get; set; }
        public string userIPAddress { get; set; }


    }
}