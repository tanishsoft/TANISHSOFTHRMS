using ScheduleApplication.DataModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Linq;
using ScheduleApplication.Common;

namespace ScheduleApplication.AttendanceUpdate
{
    public class DefaultCredentilsas
    {
        public string strInstance { get; set; }
        public string strUserName { get; set; }
        public string strPassword { get; set; }
        public string strDate { get; set; }
    }
    public class DoctorsPunchUpdate
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        string url = "http://108.163.186.66:8082/HCMWebServiceJSON/konnect/time/punchinoutentries";
        string suburl = "";

        public void UpdateDoctorsSwipeRecords()
        {
            DefaultCredentilsas postmodel = new DefaultCredentilsas();
            DateTime Dt = DateTime.Now;
            Dt = Dt.AddDays(-15);
            postmodel.strDate = Dt.ToString("yyyy-MM-dd");
            postmodel.strInstance = "fernandez";
            postmodel.strUserName = "admin";
            postmodel.strPassword = "secure@123#WR";
            try
            {
                using (var client = new HttpClient())
                {
                    List<DoctorPunchInOutModel> result = new List<DoctorPunchInOutModel>();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(webApiAuthorizationHeaderSchema, webApiAuthorizationHeaderValue);
                    client.BaseAddress = new Uri(url);
                    var httpClientResponse = client.PostAsJsonAsync(suburl, postmodel).Result;
                    if (httpClientResponse.StatusCode == HttpStatusCode.OK)
                    {
                        result = httpClientResponse.Content.ReadAsAsync<List<DoctorPunchInOutModel>>().Result;
                        foreach (var res in result)
                        {
                            try
                            {
                                int empid = int.Parse(res.strEmpCode);
                                DateTime Attdate = ProjectConvert.ConverDateStringtoDatetime(res.strPunchDate, "dd-MMM-yyyy");
                                var list = myapp.tbl_att_master_doctor.Where(l => l.employeeid == empid && l.att_date == Attdate).ToList();
                                if (list.Count == 0)
                                {
                                    tbl_att_master_doctor model = new tbl_att_master_doctor();
                                    model.att_date = Attdate;
                                    model.employeeid = empid;
                                    if (res.strPunchMode == "IN")
                                    {
                                        model.in_time = res.strPunchTime;
                                    }
                                    else if (res.strPunchMode == "OUT")
                                    {
                                        model.out_time = res.strPunchTime;
                                    }
                                    myapp.tbl_att_master_doctor.Add(model);
                                    myapp.SaveChanges();
                                }
                                else
                                {
                                    list[0].att_date = Attdate;
                                    list[0].employeeid = empid;
                                    if (res.strPunchMode == "IN")
                                    {
                                        list[0].in_time = res.strPunchTime;
                                    }
                                    else if (res.strPunchMode == "OUT")
                                    {
                                        list[0].out_time = res.strPunchTime;
                                    }
                                    myapp.SaveChanges();
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


    }
}
