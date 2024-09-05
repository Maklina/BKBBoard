using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BKBBoard.DataModel;

namespace BKBBoard.Models.AppManager
{
    public class LookupValue
    {
        BKBBoardEntities db = new BKBBoardEntities();
        public IEnumerable<SelectListItem> getMeetingType()
        {
            var query = db.meetingTypes
                        .Select(x =>
                                new
                                {
                                    value = x.id,
                                    description = x.type
                                });
            return new SelectList(query.ToList(), "value", "description");
        }
    }
}