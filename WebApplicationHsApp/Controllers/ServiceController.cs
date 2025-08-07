using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Controllers
{
    public class ChangeSurveyModel
    {
        public int Id { get; set; }
        public string Json { get; set; }
        public string Text { get; set; }
    }

    public class PostSurveyResultModel
    {
        public int postId { get; set; }
        public string surveyResult { get; set; }
    }

    public class SurveyResults
    {
        public int key { get; set; }
        public string json { get; set; }
        public string name { get; set; }
    }
    [Authorize]
    public class ServiceController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Editor()
        {
            return View();
        }
        public ActionResult Results()
        {
            return View();
        }
        public ActionResult Survey()
        {
            return View();
        }
        public JsonResult getActive()
        {
            //var db = new SessionStorage(HttpContext.Session);
            List<SurveyResults> list = new List<SurveyResults>();
            var slist = myapp.tbl_Survey.ToList();
            list = (from l in slist
                    select new SurveyResults
                    {
                        key = l.SurveyId,
                        name = l.SurveyName,
                        json = l.SurveyText
                    }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public string GetSurvey(string surveyId)
        {
            int intsurveyid = int.Parse(surveyId);
            var text = myapp.tbl_Survey.Where(l => l.SurveyId == intsurveyid).Single();
            return text.SurveyText;

        }



        public JsonResult Create(string name)
        {
            tbl_Survey ser = new tbl_Survey();
            ser.SurveyName = name;
            ser.SurveyText = "";
            ser.CreatedBy = User.Identity.Name;
            ser.CreatedOn = DateTime.Now;
            ser.ModifiedBy = User.Identity.Name;
            ser.ModifiedOn = DateTime.Now;
            myapp.tbl_Survey.Add(ser);
            myapp.SaveChanges();
            //var db = new SessionStorage(HttpContext.Session);
            //db.StoreSurvey(name, "{}");
            return Json("Ok", JsonRequestBehavior.AllowGet);
        }


        public JsonResult ChangeName(string id, string name)
        {
            int intsurveyid = int.Parse(id);
            var text = myapp.tbl_Survey.Where(l => l.SurveyId == intsurveyid).Single();
            text.SurveyName = name;
            myapp.SaveChanges();
            return Json("Ok");
        }


        public string ChangeJson(ChangeSurveyModel model)
        {
            //var db = new SessionStorage(HttpContext.Session);
            //db.StoreSurvey(model.Id, model.Json);
            //return db.GetSurvey(model.Id);

            var text = myapp.tbl_Survey.Where(l => l.SurveyId == model.Id).Single();
            text.SurveyText = model.Json;
            myapp.SaveChanges();
            return text.SurveyText;
        }


        public JsonResult Delete(string id)
        {
            int intsurveyid = int.Parse(id);
            var text = myapp.tbl_Survey.Where(l => l.SurveyId == intsurveyid).Single();
            myapp.tbl_Survey.Remove(text);
            myapp.SaveChanges();

            return Json("Ok", JsonRequestBehavior.AllowGet);
        }


        public JsonResult PostResult(PostSurveyResultModel model)
        {
            //var db = new SessionStorage(HttpContext.Session);
            //db.PostResults(model.postId, model.surveyResult);

            tbl_SurveyResult ts = new tbl_SurveyResult();
            ts.CreatedBy = User.Identity.Name;
            ts.CreatedOn = DateTime.Now;
            ts.ModifiedBy = User.Identity.Name;
            ts.ModifiedOn = DateTime.Now;
            ts.SurveyId = model.postId;
            ts.SurveyResult = model.surveyResult;
            myapp.tbl_SurveyResult.Add(ts);
            myapp.SaveChanges();
            return Json("Ok", JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetResults(string postId)
        {
            //var db = new SessionStorage(HttpContext.Session);
            //return Json(db.GetResults(postId));
            int intsurveyid = int.Parse(postId);
           
            var slist = myapp.tbl_SurveyResult.Where(l => l.SurveyId == intsurveyid).Select(l => l.SurveyResult).ToList();
         
            return Json(slist, JsonRequestBehavior.AllowGet);

        }
        //public JsonResult ExportExcelEmpDetails(int postId)
        //{
          

        //    var slist = myapp.tbl_SurveyResult.Where(l => l.SurveyId == postId).Select(l => l.SurveyResult).ToList();
        //    foreach (var v in slist)
        //    {
        //        var myDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(v);
        //    }
        //    var products = new System.Data.DataTable("Compoff");
        //    // products.Rows.Add("Client  : " + list[0].ClientName + " SOA From : " + FromDate + " To : " + ToDate);
        //    //  products.Columns.Add("User Id", typeof(string));
        //    //products.Columns.Add("User Leave Id", typeof(string));
        //    products.Columns.Add("User Id", typeof(string));
        //    products.Columns.Add("Location Name", typeof(string));
        //    products.Columns.Add("Department Name", typeof(string));
        //    products.Columns.Add("User Name", typeof(string));
        //    products.Columns.Add("CompOff Date", typeof(string));
        //    products.Columns.Add("Reason", typeof(string));
        //    products.Columns.Add("Status", typeof(string));
        //    products.Columns.Add("Created On", typeof(string));
        //    //products.Columns.Add("IsActive", typeof(string));
        //    foreach (var c in slist)
        //    {
        //        var myDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(v);

        //        products.Rows.Add(
        //                         //c.UserLeaveId.ToString(),
        //                         c.UserId.ToLower().Replace("fh_", ""),
        //                         c.LocationName,
        //                         c.DepartmentName,
        //                         c.UserName,
        //                         c.CompOffDateTime.HasValue ? c.CompOffDateTime.Value.ToString("dd/MM/yyyy") : "",
        //                         c.RequestReason,
        //                         c.Leave_Status,
        //                         c.CreatedDateTime.Value.ToString("dd/MM/yyyy")

        //        //Convert.ToBoolean(c.IsActive).ToString()
        //        );

        //    }

        //    var grid = new GridView();
        //    grid.DataSource = products;
        //    grid.DataBind();
        //    Response.ClearContent();
        //    Response.AddHeader("content-disposition", "attachement; filename=EmployeeDetails.xls");
        //    Response.ContentType = "application/excel";
        //    StringWriter sw = new StringWriter();
        //    HtmlTextWriter htw = new HtmlTextWriter(sw);
        //    grid.RenderControl(htw);
        //    Response.Output.Write(sw.ToString());
        //    Response.Flush();
        //    Response.End();
        //    return Json("Success", JsonRequestBehavior.AllowGet);
        //}
        // // GET api/values/5
        // [HttpGet("{id}")]
        // public string Get(int id)
        // {
        //     return "value";
        // }

        // // POST api/values
        // [HttpPost]
        // public void Post([FromBody]string value)
        // {
        // }

        // // PUT api/values/5
        // [HttpPut("{id}")]
        // public void Put(int id, [FromBody]string value)
        // {
        // }

        // // DELETE api/values/5
        // [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }
    }
}