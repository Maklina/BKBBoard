using BKBBoard.DataModel;
using BKBBoard.Models;
using BKBBoard.Models.AppManager;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace BKBBoard.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        BKBBoardEntities db = new BKBBoardEntities();



        public ActionResult Index(int? page)
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
                    using (BKBBoardEntities _entity = new BKBBoardEntities())
                    {
                        List<AccountInfo> allUsers = (from a in db.loginUsers
                                                                orderby a.createdBy descending
                                                                select new AccountInfo
                                                                {
                                                                    id = a.id,
                                                                    userID = a.userID, 
                                                                    userName = a.userName,
                                                                    userRole = a.userRole, 
                                                                    userDept = a.userDept, 
                                                                    isActive = a.isActive, 
                                                                    createdBy = a.createdBy, 
                                                                    createdOn = a.createdOn
                                                                }).ToList();
                        // var allUsers = _entity.loginUsers.ToList();
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
                            if (item.isActive == "1")
                            {
                                item.isActive = "Active";
                            }
                            else
                            {
                                item.isActive = "Inactive"; 
                            }
                            int id = Convert.ToInt32(item.userDept);
                            department department = _entity.departments.Where(o => o.id == id).FirstOrDefault();
                            item.userDept = department.deptName;
                            item.statusList = new SelectList(
                                new[]
                                {
                                    new SelectListItem { Value = "1", Text = "Active" ,Selected = true},
                                    new SelectListItem { Value = "0", Text = "Inactive" },
                                },
                                "Value", "Text");
                        }
                        int pageSize = 25;
                        int pageNumber = (page ?? 1);
                        return View(allUsers.ToPagedList(pageNumber, pageSize)); 
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

          public ActionResult Create()
          {
              var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
              if (login != null)
              {
                  //int rolP = Convert.ToInt32(login.role_priority);
                  AccountInfo a = new AccountInfo();
                  var queryRole = from q in db.Roles
                                  select new
                                  {
                                      value = q.id,
                                      description = q.role_name
                                  };
                var querydept = from q in db.departments
                            select new
                            {
                                value = q.id,
                                description = q.deptName
                            };
                a.statusList = new SelectList(
                                new[]
                                {
                                    new SelectListItem { Value = "1", Text = "Active" ,Selected = true},
                                    new SelectListItem { Value = "0", Text = "Inactive" },
                                },
                                "Value","Text"); 
                a.roleList = new SelectList(queryRole.ToList(), "value", "description");
                a.deptList = new SelectList(querydept.ToList(), "value", "description"); 
            

                  return View(a);
              }
              else
              {
                  return RedirectToAction("Index", "Login");
              }
          }


          [HttpPost]
          public ActionResult Create(AccountInfo a)
          {
              var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];

              loginUser LU = new loginUser();

              if (login != null)
              {
                  var userExists = db.loginUsers.Where(o => o.userID.Equals(a.userID)).FirstOrDefault();
                  if (userExists != null)
                  {
                      TempData["retMsg"] = "User ID already exists! Please try different user ID.";
                      return RedirectToAction("Create", "Account");
                  }
                  //int rolP = Convert.ToInt32(login.role_priority.ToString());
                  try
                  {
                    LU.userID = a.userID; 
                    LU.password = PasswordManager.encryption(a.userID + a.password).ToString();
                    LU.userName = a.userName;
                    LU.userRole = a.userRole;
                    LU.userDept = a.userDept;
                    LU.isActive = a.isActive;
                    LU.createdBy = login.userName;
                    LU.createdOn = DateTime.Now;

                    db.loginUsers.Add(LU);

                    db.SaveChanges();

                    TempData["retMsg"] = a.userID + " user created successfully.";
                    return RedirectToAction("Index", "Account"); 
                  }
                  catch (Exception ex)
                  {
                      TempData["errMsg"] = "Error Occured!" + ex.Message;
                      return RedirectToAction("Index", "Account");
                  }
              }
              else
              {
                  return RedirectToAction("Index", "Login");
              }
          }

        public ActionResult changeStatus(int id, string changeTo)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if(login != null)
            {
                loginUser lu = db.loginUsers.Where(o => o.id == id).FirstOrDefault();
                lu.isActive = changeTo;
                lu.updatedBy = login.UserId;
                lu.updatedOn = DateTime.Now; 
                try
                {
                    db.Entry(lu).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    TempData["errMsg"] = "Error:" + e.Message;
                    //return Json(TempData, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("Index", "Account");
                }
                return RedirectToAction("Index", "Account", new { __id = id });
            }

            else
            {
                return RedirectToAction("Index", "Login");

            }
        }

        public ActionResult ChangePassword()
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(PasswordChangeModel pcm)
        {
            var _login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if(_login != null)
            {
                string userID = _login.UserId.ToString();
                try
                {
                    string password = PasswordManager.encryption(_login.UserId.Trim() + pcm.OldPassword).ToString();
                    var isExist = db.loginUsers.Where(x => x.userID.Trim().ToLower() == _login.UserId.Trim().ToLower() && x.password.Equals(password)).SingleOrDefault();
                    if (isExist != null)
                    {
                        loginUser lu = db.loginUsers.Find(userID);
                        if (pcm.NewPassword == pcm.ConfirmPassword)
                        {
                            try
                            {
                                lu.password = PasswordManager.encryption(lu.userID + pcm.NewPassword);
                                lu.updatedBy = _login.UserId;
                                lu.updatedOn = DateTime.Now;
                                db.SaveChanges();
                                TempData["retMsg"] = "Password has been changed successfully.";
                                return RedirectToAction("ChangePassword", "Account");
                            }
                            catch(Exception e)
                            {
                                TempData["errMsg"] = "Error:" + e.Message;
                                return RedirectToAction("ChangePassword", "Account");
                            }
                        }
                        else
                        {
                            TempData["errMsg"] = "New password and confirm password not matched.";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["errMsg"] = "Old password is incorrect.";
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    TempData["errMsg"] = "Unknown Error! Please try again.";
                    return RedirectToAction("ChangePassword", "Account");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            
        }


        public ActionResult ResetPassword()
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {
                PasswordResetModel psm = new PasswordResetModel();
                var queryUsr = from q in db.loginUsers
                               select new
                               {
                                   value = q.userID,
                                   description = q.id + "--" + q.userID,

                               };

                
                psm.userList = new SelectList(queryUsr.ToList(), "value", "description");
                return View(psm);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(PasswordResetModel prm)
        {
            //return Json(prm, JsonRequestBehavior.AllowGet);

            var _login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if(_login != null)
            {
                try
                {
                    if (prm.NewPassword != prm.ConfirmPassword)
                    {
                        TempData["errMsg"] = "New Password and Confirm Password is not Same.";
                        return RedirectToAction("ResetPassword", "Account");
                    }
                    //string password = PasswordManager.encryption(prm.userID.Trim() + prm.NewPassword).ToString();
                    loginUser isExist = db.loginUsers.Where(x => x.userID == prm.userID).SingleOrDefault();
                    if (isExist != null)
                    {
                        // loginUser lu = db.loginUsers.Find(userID);
                        try
                        {
                            isExist.password = PasswordManager.encryption(prm.userID + prm.NewPassword);
                            isExist.updatedBy = _login.UserId;
                            isExist.updatedOn = DateTime.Now;
                            db.SaveChanges();
                            TempData["retMsg"] = "Password has been changed successfully.";
                            return RedirectToAction("ResetPassword", "Account");
                        }
                        catch(Exception e)
                        {
                            TempData["errMsg"] = "Error:" + e.Message;
                            return RedirectToAction("ResetPassword", "Account");
                        }
                    }
                    else
                    {
                        TempData["errMsg"] = "The new password and confirmation password do not match.";
                        return RedirectToAction("ResetPassword", "Account");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errMsg"] = "Unknown Error! Please try again.";
                    return RedirectToAction("ResetPassword", "Account");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            // int userID = Convert.ToInt32(prm.userID); 

        }



    }
}
