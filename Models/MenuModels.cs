using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic; 

namespace BKBBoard.Models
{
    public class MenuModels
    {
        public string MainMenuName { get; set; }
        public int MainMenuId { get; set; }
        public string SubMenuName { get; set; }
        public int SubMenuId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public string MenuStatus { get; set; }
    }
    public class RoleModels
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }

    public class RoleMenuModels
    {
        public RoleModels RoleItem { get; set; }
        public ICollection<MenuModels> MenuItem { get; set; }

    }
}