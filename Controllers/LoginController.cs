using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BKBBoard.Models;
using System.Web.Security;
using log4net;
using System.Reflection;
using BKBBoard.DataModel;
using log4net.Repository.Hierarchy;
using BKBBoard.Models.AppManager;
using System.Configuration;
using System.IO;

namespace BKBBoard.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(LoginModels _login) //Log check
        {
            if (ModelState.IsValid) //validating the user inputs
            {
                //var isExist = false;
                using (BKBBoardEntities _entity = new BKBBoardEntities())  // out Entity name is "SampleMenuMasterDBEntites"
                {
                    string ip = GetIp();
                    string password = PasswordManager.encryption(_login.UserId.Trim() + _login.Password).ToString();
                    var isExist = _entity.loginUsers.Where(x => x.userID.Trim().ToLower() == _login.UserId.Trim().ToLower() && x.password.Equals(password)).FirstOrDefault(); //validating the user name in tblLogin table whether the user name is exist or not
                    if (isExist != null)
                    {
                        if (isExist.isActive == "0")
                        {
                            TempData["retMsg"] = "Please Active Your User ID";
                            return View();
                        }
                        LoginModels _loginCredentials = _entity.loginUsers.Where
                            (x => x.userID.Trim().ToLower() == _login.UserId.Trim().ToLower()).Select
                            (x => new LoginModels
                            {
                                UserId = x.userID,
                                userName = x.userName,
                                RoleName = x.userRole,
                                BranchCode = x.userDept,
                                UserStatus = x.isActive
                            }).FirstOrDefault();  // Get the login user details and bind it to LoginModels class

                        FormsAuthentication.SetAuthCookie(_loginCredentials.UserId, false); // set the formauthentication cookie
                        Session["LoginCredentials"] = _loginCredentials; // Bind the _logincredentials details to "LoginCredentials" session

                        Session["UserID"] = _loginCredentials.UserId;
                        Session["userName"] = _loginCredentials.userName;
                        Session["userDept"] = _loginCredentials.BranchCode;
                        Session["userRole"] = _loginCredentials.RoleName;
                        var r = Convert.ToInt32(_loginCredentials.RoleName);
                        var userRoleName = _entity.Roles.Where
                            (o => o.id.Equals(r)).FirstOrDefault();

                        Session["userRoleName"] = userRoleName.role_name;

                        List<MenuModels> _menus = _entity.MainMenus.Select(x => new MenuModels
                        {
                            MainMenuId = x.id,
                            MainMenuName = x.main_menu,
                            RoleName = _loginCredentials.RoleName,
                            // SubMenuId = x.SubMenuId,
                            // SubMenuName = x.SubMenu.sub_menu_name,
                            ControllerName = x.menu_controller,
                            ActionName = x.menu_action,
                            // RoleId = x.UserRoleId,
                            //RoleName = x.Role.role_name,
                            MenuStatus = x.menuStatus
                        }).OrderBy(x => x.MainMenuId).ToList();//Get the Menu details from entity and bind it in MenuModels list.
                        Session["MenuMaster"] = _menus; //Bind the _menus list to MenuMaster session
                        Console.WriteLine(Session["MenuMaster"].ToString()); //write log

                        return RedirectToAction("Index", "Meeting");

                    }
                    else
                    {
                        TempData["retMsg"] = "Please enter the valid credentials!...";
                        return View();
                    }
                }
            }
            return View();
        }
        public string GetIp()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();
       
            return RedirectToAction("Index", "Login");
        }
        public FileResult DownloadManual()
        {
            var FileVirtualPath = ConfigurationManager.AppSettings["filepath"]+ "/Manual.pdf";
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }
    }
}
