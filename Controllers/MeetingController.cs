using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BKBBoard.Models;
using BKBBoard.DataModel;
using System.Data.Entity.Validation;
using System.IO;
using System.Configuration;
using BKBBoard.Models.AppManager;
using System.Data.Entity;
using PagedList; 

namespace BKBBoard.Controllers
{
    public class MeetingController : Controller
    {

        BKBBoardEntities db = new BKBBoardEntities();
        MeetingInfoManager manager = new MeetingInfoManager(); 
        string FileFolder = ConfigurationManager.AppSettings["filepath"];
        LookupValue lv = new LookupValue();

        // GET: Meeting
        public ActionResult Index(string meetingType, string Number,  int? page)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {
                if (String.IsNullOrEmpty(meetingType))
                {
                    meetingType = "BOARD MEETING";
                }
                try
                {
                    List<MeetingMasterViewModel> meeting = (from q in db.MeetingMasters
                                                            join r in db.meetingTypes on q.MeetingType equals r.id
                                                            where r.type.Equals(meetingType) 
                                                            orderby q.Date descending
                                                            select new MeetingMasterViewModel
                                                            {
                                                                ID = q.ID,
                                                                MeetingNo = q.MeetingNo,
                                                                MeetingType = r.type,
                                                                Title = q.Title,
                                                                Date = q.Date,
                                                                AgendaURL = q.AgendaURL,
                                                                MinutesURL = q.MinutesURL,
                                                                UserFileName = q.UserFileName,
                                                                UserMinuteFileName = q.UserMinuteFileName
                                                            }).ToList();

                    if (!String.IsNullOrEmpty(Number))
                    {
                        int meetingNo = Convert.ToInt32(Number);
                        meeting = meeting.Where(o => o.MeetingNo == meetingNo).ToList();
                    }
                    int pageSize = 25;
                    int pageNumber = (page ?? 1);
                    return View(meeting.ToPagedList(pageNumber, pageSize));
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
        [HttpGet]
        public ActionResult CreateMeeting()
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {
                MeetingViewModel mv = new MeetingViewModel();
                mv.meetingTypeList = lv.getMeetingType();
                return View(mv);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult CreateMeeting(MeetingViewModel m)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {


                try
                {
                    if (m.Agendafile != null)
                    {
                        int ID = Convert.ToInt32(m.mMaster.MeetingType);
                        var root = db.meetingTypes.Where(o => o.id == ID).FirstOrDefault();
                        string path = FileFolder + root.type;
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(Server.MapPath(path));
                        }
                        path = path + "/" + m.mMaster.MeetingNo;
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(Server.MapPath(path));
                        }

                        var agendaExtension = Path.GetExtension(m.Agendafile.FileName);
                        var agendaFileName = "Agenda-file-" + m.mMaster.MeetingNo + agendaExtension;
                        m.mMaster.UserFileName = m.Agendafile.FileName;
                        string minutesExtension = null, minutesFileName = null, MinutesPath = null;
                        if (m.Minutesfile != null)
                        {
                            minutesExtension = Path.GetExtension(m.Minutesfile.FileName);
                            minutesFileName = "Minutes-file-" + m.mMaster.MeetingNo + minutesExtension;
                            m.mMaster.UserMinuteFileName = m.Minutesfile.FileName;
                            MinutesPath = Path.Combine(Server.MapPath(path), minutesFileName);
                        }
                        string AgendaPath = Path.Combine(Server.MapPath(path), agendaFileName);

                        m.mMaster.AgendaURL = agendaFileName;
                        m.mMaster.MinutesURL = minutesFileName;
                        m.Agendafile.SaveAs(AgendaPath);
                        if (m.Minutesfile != null)
                        {
                            m.Minutesfile.SaveAs(MinutesPath);
                        }
                        string memoExtension, memoFileName, memoPath;
                        List<MeetingDetailViewModel> memoes = new List<MeetingDetailViewModel>();
                        for (int i = 0; i < m.MemoSubject.Count(); i++)
                        {
                            MeetingDetailViewModel memo = new MeetingDetailViewModel();
                            memo.MemoSubject = m.MemoSubject.ElementAt(i);
                            memo.Dept = m.department.ElementAt(i);
                            if (m.Memofile.ElementAt(i) != null)
                            {
                                memo.Memofile = m.Memofile.ElementAt(i);
                                memoExtension = Path.GetExtension(memo.Memofile.FileName);
                                memoFileName = "Memo-file-" + m.mMaster.MeetingNo + "-Serial-" + (i + 1) + memoExtension;
                                memo.UserMemoFileName = memo.Memofile.FileName;
                                memoPath = Path.Combine(Server.MapPath(path), memoFileName);

                                memo.memoURL = memoFileName;
                                memo.Memofile.SaveAs(memoPath);
                            }

                            memoes.Add(memo);
                        }

                        if (SaveFile(m.mMaster, memoes))
                        {
                            TempData["retMsg"] = "Uploaded Successfully !!";

                            return RedirectToAction("Index", "Meeting", new { meetingType = root.type });
                        }
                        else
                        {
                            TempData["errMsg"] = "Couldn't create the meeting!";
                            //return Json(TempData, JsonRequestBehavior.AllowGet);
                            return RedirectToAction("CreateMeeting", "Meeting");
                        }
                    }
                    else
                    {
                        TempData["errMsg"] = "File not found!";
                        //return Json(TempData, JsonRequestBehavior.AllowGet);
                        return RedirectToAction("CreateMeeting", "Meeting");
                    }
                }

                catch (Exception ex)
                {
                    TempData["errMsg"] = "Error:" + ex.Message;
                    //return Json(TempData, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("CreateMeeting", "Meeting");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            //return Json(formData, JsonRequestBehavior.AllowGet);
        }
        
         public ActionResult ViewDetails(int? id)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {

                MeetingMasterViewModel mm = new MeetingMasterViewModel();
                mm = manager.getMeetingMasterData(id);
                //return Json(mm, JsonRequestBehavior.AllowGet);
                MeetingDetailViewModel md = new MeetingDetailViewModel();
                MeetingViewModel mv = new MeetingViewModel();
                mv.mMaster = mm;
                mv.mDetails = manager.getMemoData(id);
                return View(mv);

            } 
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpGet]
        public ActionResult EditMeeting(int? id)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {

                MeetingMasterViewModel mm = new MeetingMasterViewModel();
                mm = manager.getMeetingMasterData(id);
                //return Json(mm, JsonRequestBehavior.AllowGet);
                MeetingDetailViewModel md = new MeetingDetailViewModel();
                MeetingViewModel mv = new MeetingViewModel();
                mv.mMaster = mm;
                mv.mDetails = manager.getMemoData(id);
                var query = from q in db.departments
                            select new
                            {
                                value = q.id,
                                description = q.deptName
                            };
                for (int i = 0; i< mv.mDetails.Count;i++) {
                    mv.mDetails.ElementAt(i).deptList = new SelectList(query.ToList(), "value", "description");
                }
                return View(mv);

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult EditMeeting(MeetingViewModel m)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {
                var memoFiles = m.mDetails;
                string memoExtension, memoFileName, memoPath, agendaExtension, agendaPath, agendaFileName, minutesExtension, minutesFileName, minutesPath;

                string path = FileFolder + m.mMaster.MeetingType + "/" + m.mMaster.MeetingNo;
                MeetingMasterViewModel mm = new MeetingMasterViewModel();
                MeetingMaster mmdb = db.MeetingMasters.Find(m.mMaster.ID);
                var transaction = db.Database.BeginTransaction();
                if (m.agendaChecked == true)
                {
                   // try
                    //{
                        mm.Agendafile = m.Agendafile;
                        agendaExtension = Path.GetExtension(mm.Agendafile.FileName);
                        agendaFileName = "Agenda-file-" + m.mMaster.MeetingNo + agendaExtension;

                        agendaPath = Path.Combine(Server.MapPath(path), agendaFileName);
                        mm.AgendaURL = agendaFileName;
                        if (System.IO.File.Exists(agendaPath))
                        {
                            System.IO.File.Delete(agendaPath);
                        }
                        mm.Agendafile.SaveAs(agendaPath);
                        mmdb.AgendaURL = mm.AgendaURL;
                        mmdb.UserFileName = m.Agendafile.FileName;
                        mmdb.UpdatedBy = login.userName;
                        mmdb.UpdatedOn = DateTime.Now;
                        db.Entry(mmdb).State = EntityState.Modified;
                        db.SaveChanges();
                    /*}
                    catch (Exception e)
                    {
                        TempData["errMsg"] = "Error:" + e.Message;
                        //return Json(TempData, JsonRequestBehavior.AllowGet);
                        return RedirectToAction("EditMeeting", "Meeting");
                    }*/


                }
                if (m.minutesChecked == true)
                {
                    //try
                    //{
                        mm.Minutesfile = m.Minutesfile;
                        minutesExtension = Path.GetExtension(mm.Minutesfile.FileName);
                        minutesFileName = "Minutes-file-" + m.mMaster.MeetingNo + minutesExtension;
                        m.mMaster.UserMinuteFileName = m.Minutesfile.FileName;

                        minutesPath = Path.Combine(Server.MapPath(path), minutesFileName);
                        mm.MinutesURL = minutesFileName;
                        if (System.IO.File.Exists(minutesPath))
                        {
                            System.IO.File.Delete(minutesPath);
                        }
                        mm.Minutesfile.SaveAs(minutesPath);
                        mmdb.MinutesURL = mm.MinutesURL;
                        mmdb.UserMinuteFileName = m.Minutesfile.FileName;
                        mmdb.UpdatedBy = login.userName;
                        mmdb.UpdatedOn = DateTime.Now;
                        db.Entry(mmdb).State = EntityState.Modified;
                        db.SaveChanges();
                   /* }
                    catch (Exception e)
                    {
                        TempData["errMsg"] = "Error:" + e.Message;
                        //return Json(TempData, JsonRequestBehavior.AllowGet);
                        return RedirectToAction("EditMeeting", "Meeting");
                    }*/
                }
                int j = 0;
                for (int i = 0; i < m.mDetails.Count(); i++)
                {
                    if (m.mDetails[i].isChecked == true)
                    {
                        try
                        {
                            MeetingDetailViewModel memo = new MeetingDetailViewModel();
                            MeetingDetail md = new MeetingDetail();
                            int id = m.mDetails[i].ID; 
                            md = db.MeetingDetails.Where(o => o.ID.Equals(id)).FirstOrDefault(); 
                           // md = db.MeetingDetails.Find(m.mDetails[i].ID);

                            //memo.MeetingID = checkedMemo.MeetingID;
                            // memo.MemoNo = checkedMemo.MemoNo;

                            md.Subject = m.mDetails[i].MemoSubject;
                            md.Dept = m.mDetails[i].Dept;

                            memo.Memofile = m.Memofile.ElementAt(j);

                            memoExtension = Path.GetExtension(memo.Memofile.FileName);
                            memoFileName = "Memo-file-" + m.mMaster.MeetingNo + "-Serial-" + (md.MemoSerial) + memoExtension;

                            memoPath = Path.Combine(Server.MapPath(path), memoFileName);

                            memo.memoURL = memoFileName;
                            if (System.IO.File.Exists(memoPath))
                            {
                                System.IO.File.Delete(memoPath);
                            }
                            memo.Memofile.SaveAs(memoPath);
                            md.memoURL = memoFileName;
                            md.UserMemoFileName = memo.Memofile.FileName;
                            md.UpdatedBy = login.userName;
                            md.UpdatedOn = DateTime.Now;
                            try
                            {
                                db.Entry(md).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                TempData["errMsg"] = "Error:" + e.Message;
                                //return Json(TempData, JsonRequestBehavior.AllowGet);
                                return RedirectToAction("EditMeeting", "Meeting");
                            }
                            j++;

                        }

                        catch (Exception e)
                        {
                            TempData["errMsg"] = "Error:" + e.Message;
                            //return Json(TempData, JsonRequestBehavior.AllowGet);
                            return RedirectToAction("EditMeeting", "Meeting");
                        }

                    }
                }
                transaction.Commit();
                return RedirectToAction("ViewDetails", "Meeting", new { id = m.mMaster.ID });
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        private bool SaveFile(MeetingMasterViewModel m, List<MeetingDetailViewModel> mdm)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            var transaction = db.Database.BeginTransaction();
            try
            {
                MeetingMaster mm = new MeetingMaster();
                mm.MeetingNo = m.MeetingNo;
                mm.MeetingType = Convert.ToInt32(m.MeetingType);
                mm.Title = m.Title;
                mm.Date = m.Date;
                mm.AgendaURL = m.AgendaURL;
                mm.MinutesURL = m.MinutesURL == null ? " " : m.MinutesURL;
                mm.CreatedBy = login.userName;
                mm.CreatedOn = DateTime.Now;
                mm.UserFileName = m.UserFileName;
                mm.UserMinuteFileName = m.UserMinuteFileName;

                db.MeetingMasters.Add(mm);
                db.SaveChanges();

                int meetingType = Convert.ToInt32(m.MeetingType);
                var meeting = db.MeetingMasters.Where(o => o.MeetingNo.Equals(m.MeetingNo) && o.MeetingType.Equals(meetingType)).SingleOrDefault();
                int i = 1;
                foreach (var item in mdm)
                {
                    MeetingDetail md = new MeetingDetail();
                    md.MeetingID = meeting.ID;
                    md.MemoSerial = i;
                    md.Subject = item.MemoSubject;
                    md.Dept = item.Dept;
                    md.memoURL = item.memoURL;
                    md.CreatedBy = login.UserId;
                    md.CreatedOn = DateTime.Now;
                    md.UserMemoFileName = item.UserMemoFileName;
                    db.MeetingDetails.Add(md);
                    i++;
                }
                db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        TempData["AlertMessage"] = validationError.PropertyName + "--" + validationError.ErrorMessage;
                    }
                }
                transaction.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                TempData["AlertMessage"] = ex.Message;
                return false;
            }
        }
        public FileResult Download(String MeetingType, int MeetingNo, String fileName)
        {
            var FileVirtualPath = FileFolder + MeetingType + "/" + MeetingNo + "/" + fileName;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }
        public JsonResult getType()
        {
            var ddlType = db.meetingTypes.ToList();
            List<SelectListItem> liType = new List<SelectListItem>();

            if (ddlType != null)
            {
                foreach (var x in ddlType)
                {
                    liType.Add(new SelectListItem { Text = x.type, Value = x.id.ToString() });
                }
            }
            return Json(liType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getDepartment()
        {
            var ddlDept = db.departments.ToList();
            List<SelectListItem> deptList = new List<SelectListItem>();

            if (ddlDept != null)
            {
                foreach (var x in ddlDept)
                {
                    deptList.Add(new SelectListItem { Text = x.deptName, Value = x.id.ToString() });
                }
            }
            return Json(deptList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getNoAcdType(String type)
        {
            var query = db.meetingTypes.Where(o=>o.type==type).FirstOrDefault();
            var t = query.id; 
            var ddlNo = db.MeetingMasters.Where(o=>o.MeetingType==t).ToList();
            List<SelectListItem> numList = new List<SelectListItem>();

            if (ddlNo != null)
            {
                foreach (var x in ddlNo)
                {
                    numList.Add(new SelectListItem { Text = x.MeetingNo.ToString(), Value = x.MeetingNo.ToString() });
                }
            }
            return Json(numList, JsonRequestBehavior.AllowGet);
        }
       

    }
}