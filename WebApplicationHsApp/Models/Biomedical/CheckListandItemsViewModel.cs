using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp
{
    public class CheckListandItemsViewModel
    {
        public tbl_bm_CheckList checkList { get; set; }
        public List<tbl_bm_CheckListItem> checkListItems { get; set; }
    }
}