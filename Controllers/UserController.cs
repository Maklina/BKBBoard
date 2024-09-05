using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BKBBoard.Models;
using BKBBoard.DataModel;

namespace BKBBoard.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            LoginModels logIn = new LoginModels();
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null && login.RoleName == "1")
            {
                try
                {
                    /* var MenuMaster = (List<BKBBoard.Models.MenuModels>)Session["MenuMaster"];
                     var loginDetails = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
                     var groupByMenu = MenuMaster.Where(x => x.MenuStatus == "3").ToList();*/
                    using (BKBBoardEntities1 _entity = new BKBBoardEntities1())
                    {
                        var allUsers = _entity.loginUsers.ToList();
                        foreach (var item in allUsers)
                        {
                            if (item.userRole == "1")
                            {
                                item.userRole = "Administrator";
                            }
                            else if (item.userRole == "2")
                            {
                                item.userRole = "Creator";
                            }
                            else
                            {
                                item.userRole = "Viewer";
                            }
                        }
                        //return Json(allUsers, JsonRequestBehavior.AllowGet);
                        return View(allUsers);
                    }

                }
                catch (Exception ex)
                {
                    TempData["errMsg"] = "Error:" + ex.Message;
                    return RedirectToAction("Index", "Login");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }
    }
}