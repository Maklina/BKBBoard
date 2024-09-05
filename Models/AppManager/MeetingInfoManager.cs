using BKBBoard.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BKBBoard.Models.AppManager
{

    public class MeetingInfoManager
    {
        BKBBoardEntities db = new BKBBoardEntities();

        public MeetingMasterViewModel getMeetingMasterData(int? id)
        {
           
            var result = from i in db.MeetingMasters
                         join j in db.meetingTypes on i.MeetingType equals j.id

                         where i.ID == id
                         select new MeetingMasterViewModel
                         {
                             ID = i.ID,
                             MeetingNo = i.MeetingNo, 
                             Title = i.Title, 
                             Date = i.Date, 
                             MeetingType = j.type, 
                             CreatedBy = i.CreatedBy, 
                             CreatedOn = i.CreatedOn, 
                             UpdatedBy = i.UpdatedBy, 
                             UpdatedOn = i.UpdatedOn, 
                             AgendaURL = i.AgendaURL,
                             MinutesURL = i.MinutesURL, 
                             UserFileName = i.UserFileName, 
                             UserMinuteFileName = i.UserMinuteFileName
                             
                             
                         };
            MeetingMasterViewModel m = result.FirstOrDefault();
            return m;
        }
        public List<MeetingDetailViewModel> getMemoData(int? id)
        {

            var result = from i in db.MeetingDetails
                         join j in db.departments on i.Dept equals j.id
                         where i.MeetingID == id
                         select new MeetingDetailViewModel
                         {
                             ID = i.ID,
                             MeetingID = i.MeetingID,
                             memoURL = i.memoURL, 
                             MemoNo = i.MemoSerial, 
                             MemoSubject = i.Subject, 
                             Dept = j.id, 
                             deptName = j.deptName, 
                             UserMemoFileName = i.UserMemoFileName
                         };
            List<MeetingDetailViewModel> memoList = result.ToList();
            return memoList;
        }
    }
}