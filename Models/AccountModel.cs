using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BKBBoard.Models
{
    public class AccountInfo
    {
        public int id { get; set; }
        [Display(Name = "User ID")]
        public string userID { get; set; }
        [Display(Name = "Password")]
        public string password { get; set; }
        [Display(Name = "User Name")]
        public string userName { get; set; }
        [Display(Name = "Role")]
        public string userRole { get; set; }
        [Display(Name = "Department")]
        public string userDept { get; set; }
        public string deptName { get; set; }
        public string roleName { get; set;  }
        [Display(Name = "Status")]
        public string isActive { get; set; }
        [Display(Name = "Created By")]
        public string createdBy { get; set; }
        [Display(Name = "Created On")]
        public Nullable<System.DateTime> createdOn { get; set; }
        public IEnumerable<SelectListItem> roleList { get; set; }
        public IEnumerable<SelectListItem> deptList { get; set; }
        public IEnumerable<SelectListItem> statusList { get; set; }

    }
    public class PasswordChangeModel
    {
        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class PasswordResetModel
    {
        [Required(ErrorMessage = "Required")]
        [Display(Name = "User ID")]
        public string userID { get; set; }

        public IEnumerable<SelectListItem> userList { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}