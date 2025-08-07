using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.OracleInterface
{
    public class ConnectOracle
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        string Evironmentcheck = ConfigurationManager.AppSettings["Environment"];
        string host = "172.16.0.7";
        string port = "1521";
        string user = "infonet";
        string password = "infonet";
        //string sid = "neosoft";
        string sid = "mirrorfh";
        public List<emp_details> GetEmployees()
        {
            string Messagestatus = "Success";

            string oradb = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = " + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = " + sid + ")));Password=" + password + ";User ID=" + user;
            OracleConnection con = new OracleConnection(oradb);
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "select * from empinfo_read where retirementflag != 1";
                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    List<emp_details> listemp = new List<emp_details>();
                    var userslist = myapp.tbl_User.ToList();
                    while (dr.Read())
                    {
                        var empnum = dr["empcode"] != null ? dr["empcode"].ToString() : "";
                        var checkcount = userslist.Where(l => l.CustomUserId.Replace("FH_", "") == empnum).Count();
                        if (checkcount == 0)
                        {
                            emp_details obj = new emp_details();
                            obj.empnm = dr["empnm"] != null ? dr["empnm"].ToString() : "";
                            obj.empcode = dr["empcode"] != null ? dr["empcode"].ToString() : "";
                            obj.sdeptid = dr["sdeptid"] != null ? dr["sdeptid"].ToString() : "";
                            obj.designationid = dr["designationid"] != null ? dr["designationid"].ToString() : "";
                            obj.address = dr["address"] != null ? dr["address"].ToString() : "";
                            obj.area = dr["area"] != null ? dr["area"].ToString() : "";
                            obj.city = dr["city"] != null ? dr["city"].ToString() : "";
                            obj.state = dr["state"] != null ? dr["state"].ToString() : "";
                            obj.pincode = dr["pincode"] != null ? dr["pincode"].ToString() : "";
                            obj.phno = dr["phno"] != null ? dr["phno"].ToString() : "";
                            obj.doj = dr["doj"] != null ? dr["doj"].ToString() : "";
                            obj.dob = dr["dob"] != null ? dr["dob"].ToString() : "";
                            obj.sex = dr["sex"] != null ? dr["sex"].ToString() : "";
                            obj.companynm = dr["companynm"] != null ? dr["companynm"].ToString() : "";
                            obj.companyid = dr["companyid"] != null ? dr["companyid"].ToString() : "";
                            obj.grade = dr["grade"] != null ? dr["grade"].ToString() : "";
                            obj.bldgrp = dr["bldgrp"] != null ? dr["bldgrp"].ToString() : "";
                            obj.emptype = dr["emptype"] != null ? dr["emptype"].ToString() : "";
                            obj.martialstatus = dr["martialstatus"] != null ? dr["martialstatus"].ToString() : "";
                            obj.relativename = dr["relativename"] != null ? dr["relativename"].ToString() : "";
                            obj.emailid = dr["emailid"] != null ? dr["emailid"].ToString() : "";
                            obj.locid = dr["locid"] != null ? dr["locid"].ToString() : "";
                            obj.branch = dr["branch"] != null ? dr["branch"].ToString() : "";
                            obj.emergencyno = dr["emergencyno"] != null ? dr["emergencyno"].ToString() : "";
                            listemp.Add(obj);
                        }
                    }
                    con.Close();
                    return listemp;
                    //Response.Write("</table>");  
                }
                else
                {
                    return new List<emp_details>();
                }
            }
            catch (Exception ex)
            {
                Messagestatus = "Error " + ex.Message;
            }

            return new List<emp_details>();
        }

        public string Getdesignation()
        {
            string message = "Success";
            string oradb = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = " + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = " + sid + ")));Password=" + password + ";User ID=" + user;
            OracleConnection con = new OracleConnection(oradb);
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "select * from designation";
                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //Response.Write("<table border='1'>");  
                    //Response.Write("<tr><th>Name</th><th>Roll No</th></tr>"); 
                    List<designation> listemp = new List<designation>();
                    while (dr.Read())
                    {
                        designation obj = new designation();
                        obj.designationid = dr["designationid"] != null ? dr["designationid"].ToString() : "";
                        obj.deptid = dr["deptid"] != null ? dr["deptid"].ToString() : "";
                        obj.designationnm = dr["designationnm"] != null ? dr["designationnm"].ToString() : "";
                        listemp.Add(obj);
                    }
                    foreach (var l in listemp)
                    {
                        string name = l.designationnm.ToLower().Replace(" ", "");
                        var checkexists = myapp.tbl_MasterEmployeeDesignation.Where(d => d.Designation_Name.ToLower().Replace(" ", "") == name).ToList();
                        if (checkexists.Count > 0)
                        {
                            checkexists[0].designationid = l.designationid;
                            myapp.SaveChanges();
                        }
                        else
                        {
                            tbl_MasterEmployeeDesignation model = new tbl_MasterEmployeeDesignation();
                            model.CreatedBy = "Admin";
                            model.CreatedDateTime = DateTime.Now;
                            model.CreatedOn = DateTime.Now.ToString("dd/MM/yyyy");
                            model.designationid = l.designationid;
                            model.Designation_Name = l.designationnm;
                            model.Record_Status = true;
                            myapp.tbl_MasterEmployeeDesignation.Add(model);
                            myapp.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Error " + ex.Message;
            }
            con.Close();
            return message;
        }
        public string Getsubdepts()
        {
            string message = "Success";
            string oradb = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = " + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = " + sid + ")));Password=" + password + ";User ID=" + user;
            OracleConnection con = new OracleConnection(oradb);
            OracleCommand cmd = new OracleCommand();
            try
            {
                cmd.CommandText = "select * from subdepts";
                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //Response.Write("<table border='1'>");  
                    //Response.Write("<tr><th>Name</th><th>Roll No</th></tr>"); 
                    List<subdepts> listemp = new List<subdepts>();

                    while (dr.Read())
                    {
                        subdepts obj = new subdepts();
                        obj.sdeptid = dr["sdeptid"] != null ? dr["sdeptid"].ToString() : "";
                        obj.deptid = dr["deptid"] != null ? dr["deptid"].ToString() : "";
                        obj.subdeptnm = dr["subdeptnm"] != null ? dr["subdeptnm"].ToString() : "";
                        listemp.Add(obj);
                    }
                    foreach (var l in listemp)
                    {
                        string name = l.subdeptnm.ToLower().Replace(" ", "");
                        var checkexists = myapp.tbl_Oracle_subdepts.Where(d => d.subdeptnm.ToLower().Replace(" ", "") == name).ToList();
                        if (checkexists.Count == 0)
                        {
                            tbl_Oracle_subdepts model = new tbl_Oracle_subdepts();
                            model.sdeptid = l.sdeptid;
                            model.deptid = l.deptid;
                            model.subdeptnm = l.subdeptnm;
                            myapp.tbl_Oracle_subdepts.Add(model);
                            myapp.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Error " + ex.Message;
            }
            con.Close();
            return message;
        }
        public string Getcompany()
        {
            string message = "Success";
            string oradb = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = " + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = " + sid + ")));Password=" + password + ";User ID=" + user;
            OracleConnection con = new OracleConnection(oradb);
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "select * from company";
                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //Response.Write("<table border='1'>");  
                    //Response.Write("<tr><th>Name</th><th>Roll No</th></tr>"); 
                    List<company> listemp = new List<company>();

                    while (dr.Read())
                    {
                        company obj = new company();
                        obj.companyid = dr["companyid"] != null ? dr["companyid"].ToString() : "";
                        obj.companynm = dr["companynm"] != null ? dr["companynm"].ToString() : "";
                        obj.locid = dr["locid"] != null ? dr["locid"].ToString() : "";
                        listemp.Add(obj);
                    }

                    foreach (var l in listemp)
                    {
                        string name = l.companynm.ToLower().Replace(" ", "");
                        var checkexists = myapp.tbl_Location.Where(d => d.LocationName.ToLower().Replace(" ", "") == name).ToList();
                        if (checkexists.Count > 0)
                        {
                            checkexists[0].locid = l.locid;
                            checkexists[0].companyid = l.companyid;
                            myapp.SaveChanges();
                        }
                        else
                        {
                            tbl_Location model = new tbl_Location();
                            model.companyid = l.companyid;
                            model.locid = l.locid;
                            model.LocationName = l.companynm;
                            myapp.tbl_Location.Add(model);
                            myapp.SaveChanges();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                message = "Error " + ex.Message;
            }
            finally
            {
                con.Close();
            }
            return message;
        }

        public List<Items_Purchase> UpdateStock(string ponumber)
        {

            string message = "Success";
            List<Items_Purchase> listemp = new List<Items_Purchase>();
            string oradb = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = " + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = " + sid + ")));Password=" + password + ";User ID=" + user;
            OracleConnection con = new OracleConnection(oradb);
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "select purchdt,pono,(select mednm from inventory.medmast where  medmast.medid=purchasedtlo.itemid) itemname, inventory.purchasedtlo.qty,inventory.purchasedtlo.rate,purchasemasto.locid from inventory.purchasemasto,inventory.purchasedtlo where purchasemasto.purchid = purchasedtlo.purchid and pono = '" + ponumber + "'";
                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //tbl_cm_ReceivedStock model = new tbl_cm_ReceivedStock();
                    //var checkdata = myapp.tbl_cm_ReceivedStock.Where(l => l.StockRecivedNotes == ponumber).ToList();
                    //if (checkdata.Count == 0)
                    //{
                    //    model.CreatedBy = userid;
                    //    model.CreatedOn = DateTime.Now;
                    //    model.IsActive = true;
                    //    model.ModifiedBy = userid;
                    //    model.ModifiedOn = DateTime.Now;
                    //    model.StockRecivedEmpId = int.Parse(userid);
                    //    model.StockRecivedNotes = ponumber;
                    //    model.StockRecivedOn = DateTime.Now;
                    //    model.StoreId = myapp.tbl_cm_Store.Where(l => l.StoreName.Contains("Main")).SingleOrDefault().StoreId;
                    //    model.TotalItems = 0;
                    //    model.TotalItemsAccepted = 0;
                    //    model.TotalItemsDamage = 0;
                    //    model.TotalItemsReturn = 0;
                    //    myapp.tbl_cm_ReceivedStock.Add(model);
                    //    myapp.SaveChanges();
                    //}
                    //else
                    //{
                    //    model = checkdata[0];
                    //}
                    //message = model.ReceivedStockId.ToString();

                    while (dr.Read())
                    {
                        Items_Purchase obj = new Items_Purchase();
                        obj.purchdt = dr["purchdt"] != null ? dr["purchdt"].ToString() : "";
                        obj.itemname = dr["itemname"] != null ? dr["itemname"].ToString() : "";
                        obj.qty = dr["qty"] != null ? dr["qty"].ToString() : "";
                        obj.pono = dr["pono"] != null ? dr["pono"].ToString() : "";
                        obj.rate = dr["rate"] != null ? dr["rate"].ToString() : "";
                        obj.locid = dr["locid"] != null ? dr["locid"].ToString() : "";
                        listemp.Add(obj);
                        //tbl_cm_ReceivedStockItem mt = new tbl_cm_ReceivedStockItem();
                        //mt.ReceivedStockId = model.ReceivedStockId;
                        //mt.QtyRecived = int.Parse(obj.qty);
                        //mt.QtyDamage = 0;
                        //mt.QtyRequested = 0;
                        //mt.QtyReturn = 0;
                        //mt.QtySend = 0;
                        //mt.Notes = obj.purchdt + obj.itemname;
                        //mt.IsActive = true;
                        //var itemcheck = myapp.tbl_cm_Item.Where(l => l.ItemName == obj.itemname).SingleOrDefault();
                        //if (itemcheck != null)
                        //{
                        //    mt.ItemId = itemcheck.ItemId;
                        //}
                        //else
                        //{
                        //    tbl_cm_Item item = new tbl_cm_Item();
                        //    item.CostPerServe = 0;
                        //    item.CreatedBy = userid;
                        //    item.CreatedOn = DateTime.Now;
                        //    item.DeliveryCost = 0;
                        //    item.HeatingInstructions = "";
                        //    item.IngredientTotalCost = 0;
                        //    item.IsActive = true;
                        //    item.IsRecipe = true;
                        //    item.ItemCode = "";
                        //    item.ItemDescription = obj.itemname;
                        //    item.ItemName = obj.itemname;
                        //    item.ItemType = "Recipe";
                        //    item.KitchenNotes = "";
                        //    item.LabourCost = 0;
                        //    item.ModifiedBy = userid;
                        //    item.ModifiedOn = DateTime.Now;
                        //    item.Notes = "";
                        //    item.OtherCost = 0;
                        //    item.Packaging = "";
                        //    item.PackagingCost = 0;
                        //    item.PackagingInstructions = "";
                        //    item.PreparationMethod = "";
                        //    item.Serves = 1;
                        //    item.ServingInstructions = "";
                        //    item.TotalCalories = 1;
                        //    item.TotalCost = Convert.ToDecimal(obj.rate);
                        //    item.TotalCostStaff = Convert.ToDecimal(obj.rate);
                        //    item.UnitTypeId = 1;
                        //    myapp.tbl_cm_Item.Add(item);
                        //    myapp.SaveChanges();
                        //    mt.ItemId = item.ItemId;
                        //}

                        //var checkdatareciveditems = myapp.tbl_cm_ReceivedStockItem.Where(l1 => l1.ReceivedStockId == model.ReceivedStockId && l1.ItemId == mt.ItemId).Count();
                        //if (checkdatareciveditems == 0)
                        //{
                        //    myapp.tbl_cm_ReceivedStockItem.Add(mt);
                        //    myapp.SaveChanges();
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Error " + ex.Message;
            }
            con.Close();
            return listemp;
        }

        public string UpdatePatientInfo()
        {
            string Datecheck = DateTime.Now.AddYears(-2).ToString("yyyy/MM/dd");

            string message = "Success";
            string oradb = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = " + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = " + sid + ")));Password=" + password + ";User ID=" + user;
            OracleConnection con = new OracleConnection(oradb);
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "select  to_char(regdt,'dd/mm/yyyy') regdt,to_char(regtm,'hh:mi:ss AM')regtm,ipno,Casesheetnumber mrno,i.suffix || name name, I.Patid,AGE,i.Phno2 as phno,i.currentbed BedId, bb.bedno,(select bedno from ip.beds b where b.bedid = i.currentbed)currnetbed,DISCHARGEDFLG, (select r.roomno from ip.rooms r where R.Roomid = (select B.Roomid from ip.beds b where b.bedid = Bb.Bedid))RoomNo, 1 FloorId, BB.ROOMID RoomId, (select Rr.Roomtypes from ip.roomtype rr where Rr.Roomtypeid = (select R.Roomtypeid from ip.rooms r where R.Roomid = (select B.Roomid from ip.beds b where b.bedid = Bb.Bedid)))RoomType ,(select F.Floorno from ip.floors f where F.Floorid = (select R.Floorid from ip.rooms r where R.Roomid = (select B.Roomid from ip.beds b where b.bedid = Bb.Bedid)))FloorNo, (select L.Locnm from IP.Locationmast l where l.locid = I.Locid)LocationName, i.locid LocationId from ip.ipinfo i join ip.beds bb on I.Patid = bb.Patid where BB.PATID IS not NULL and i.locid is not null order by REGDT";
                cmd.Connection = con;

                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    List<tbl_Patient> listpat = new List<tbl_Patient>();
                    int rowscount = 0;
                    while (dr.Read())
                    {
                        if (rowscount == 0)
                        {
                            int noOfRowDeleted = myapp.Database.ExecuteSqlCommand("update tbl_Patient set Bedallocated=0");
                        }
                        rowscount++;
                        string ipnumber = dr["ipno"] != null ? dr["ipno"].ToString() : "";
                        string MRNo = dr["MRNo"] != null ? dr["MRNo"].ToString() : "";
                        string floorno = dr["FloorNo"] != null ? dr["FloorNo"].ToString() : "";
                        var checkpatient = myapp.tbl_Patient.Where(l => l.IPNo == ipnumber && l.MRNo == MRNo && l.FloorNo == floorno).ToList();
                        if (checkpatient.Count > 0)
                        {
                            checkpatient[0].Bedallocated = true;
                            checkpatient[0].Regdt = dr["regdt"] != null ? dr["regdt"].ToString() : "";
                            checkpatient[0].Regtm = dr["regtm"] != null ? dr["regtm"].ToString() : "";
                            checkpatient[0].MRNo = dr["mrno"] != null ? dr["mrno"].ToString() : "";
                            checkpatient[0].IPNo = ipnumber;
                            checkpatient[0].Name = dr["name"] != null ? dr["name"].ToString() : "";
                            checkpatient[0].Age = dr["AGE"] != null ? dr["AGE"].ToString() : "";
                            checkpatient[0].MobileNumber = dr["PHNO"] != null ? dr["PHNO"].ToString() : "";
                            checkpatient[0].BedNo = dr["BedNo"] != null ? dr["BedNo"].ToString() : "";
                            checkpatient[0].BedId = dr["BedId"] != null ? dr["BedId"].ToString() : "";
                            checkpatient[0].RoomType = dr["RoomType"] != null ? dr["RoomType"].ToString() : "";
                            checkpatient[0].RoomId = dr["RoomId"] != null ? dr["RoomId"].ToString() : "";
                            checkpatient[0].RoomNo = dr["RoomNo"] != null ? dr["RoomNo"].ToString() : "";
                            checkpatient[0].FloorId = dr["FloorId"] != null ? dr["FloorId"].ToString() : "";
                            checkpatient[0].FloorNo = floorno;
                            checkpatient[0].LocationId = dr["LocationId"] != null ? dr["LocationId"].ToString() : "";
                            checkpatient[0].LocationName = dr["LocationName"] != null ? dr["LocationName"].ToString() : "";
                            if (checkpatient[0].RoomId == "R0000045" || checkpatient[0].RoomId == "R0000011" || checkpatient[0].RoomId == "R0000046" ||
                                checkpatient[0].RoomId == "R0000066")
                            {
                                checkpatient[0].Remarks = "LW";
                            }
                            else if (checkpatient[0].RoomId == "R0000040" || checkpatient[0].RoomId == "R0000089" || checkpatient[0].RoomId == "R0000008" ||
                             checkpatient[0].RoomId == "R0000063")
                            {
                                checkpatient[0].Remarks = "POW";
                            }
                            else
                            {
                                checkpatient[0].Remarks = "Normal";
                            }
                            myapp.SaveChanges();
                        }
                        else
                        {
                            tbl_Patient obj = new tbl_Patient();
                            obj.Regdt = dr["regdt"] != null ? dr["regdt"].ToString() : "";
                            obj.Regtm = dr["regtm"] != null ? dr["regtm"].ToString() : "";
                            obj.MRNo = dr["mrno"] != null ? dr["mrno"].ToString() : "";
                            obj.IPNo = ipnumber;
                            obj.Name = dr["name"] != null ? dr["name"].ToString() : "";
                            obj.Age = dr["AGE"] != null ? dr["AGE"].ToString() : "";
                            obj.MobileNumber = dr["PHNO"] != null ? dr["PHNO"].ToString() : "";
                            obj.BedNo = dr["BedNo"] != null ? dr["BedNo"].ToString() : "";
                            obj.BedId = dr["BedId"] != null ? dr["BedId"].ToString() : "";
                            obj.RoomType = dr["RoomType"] != null ? dr["RoomType"].ToString() : "";
                            obj.RoomId = dr["RoomId"] != null ? dr["RoomId"].ToString() : "";
                            obj.RoomNo = dr["RoomNo"] != null ? dr["RoomNo"].ToString() : "";
                            obj.FloorId = dr["FloorId"] != null ? dr["FloorId"].ToString() : "";
                            obj.FloorNo = floorno;
                            obj.LocationId = dr["LocationId"] != null ? dr["LocationId"].ToString() : "";
                            obj.LocationName = dr["LocationName"] != null ? dr["LocationName"].ToString() : "";

                            obj.Bedallocated = true;
                            obj.Address = "";
                            obj.Area = "";
                            obj.CreatedBy = "System";
                            obj.CreatedOn = DateTime.Now;
                            obj.ModifiedBy = "System";
                            obj.ModifiedOn = DateTime.Now;
                            if (obj.RoomId == "R0000045" || obj.RoomId == "R0000011" || obj.RoomId == "R0000046" || obj.RoomId == "R0000066")
                            {
                                obj.Remarks = "LW";
                            }
                            else if (obj.RoomId == "R0000040" || obj.RoomId == "R0000089" || obj.RoomId == "R0000008" || obj.RoomId == "R0000063")
                            {
                                obj.Remarks = "POW";
                            }
                            else if (obj.RoomId == "R0000083" || obj.RoomId == "R0000043" || obj.RoomId == "R0000065")
                            {
                                obj.Remarks = "HDU";
                            }
                            else
                            {
                                obj.Remarks = "Normal";
                            }
                            myapp.tbl_Patient.Add(obj);
                            myapp.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Error " + ex.Message;
            }
            finally
            {
                con.Close();
            }

            return message;
        }

        public Patient_Billing GetBillingDetails(string ipno)
        {
            Patient_Billing patient = new Patient_Billing();
            string oradb = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = " + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = " + sid + ")));Password=" + password + ";User ID=" + user;
            OracleConnection con = new OracleConnection(oradb);
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "select b.patid,i.ipno,i.name,i.regdt admissiondate,i.dischargedt,b.total totalamount,b.discount,B.NETAMOUNT,(select sum(m.AMOUNT) from IP.MONEYRECEIPT m where m.patid = i.patid) as paidamount,b.balance,(select nvl(sum(p.DISCAMT), 0) from IP.IPDPOSTDISC p where p.PATID = i.patid) postdisc from ip.ipinfo I JOIN ip.billhdr B ON I.PATID = B.PATID where nvl(I.BILLEDFLG,0)= 1 and dischargedflg = '1' and ipno = '" + ipno + "'";
                cmd.Connection = con;
                //int noOfRowDeleted = myapp.Database.ExecuteSqlCommand("update tbl_Patient set Bedallocated=0");
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();                
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        patient.patid = dr["patid"] != null ? dr["patid"].ToString() : "";
                        patient.ipno = dr["ipno"] != null ? dr["ipno"].ToString() : "";
                        patient.admissiondate = dr["admissiondate"] != null ? dr["admissiondate"].ToString() : "";
                        patient.dischargedt = dr["dischargedt"] != null ? dr["dischargedt"].ToString() : "";
                        patient.totalamount = dr["totalamount"] != null ? dr["totalamount"].ToString() : "";
                        patient.paidamount = dr["paidamount"] != null ? dr["paidamount"].ToString() : "";
                        patient.discount = dr["discount"] != null ? dr["discount"].ToString() : "";
                        patient.balance = dr["balance"] != null ? dr["balance"].ToString() : "";
                        patient.postdisc = dr["postdisc"] != null ? dr["postdisc"].ToString() : "";
                        patient.name = dr["name"] != null ? dr["name"].ToString() : "";
                        patient.NETAMOUNT = dr["NETAMOUNT"] != null ? dr["NETAMOUNT"].ToString() : "";
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Error " + ex.Message;
            }
            finally
            {
                con.Close();
            }
            return patient;
        }
        public patient_details GetPatientBasicDetails(string ipno,string mrno)
        {
            patient_details patient = new patient_details();
            string oradb = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = " + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = " + sid + ")));Password=" + password + ";User ID=" + user;
            OracleConnection con = new OracleConnection(oradb);
            try
            {
                OracleCommand cmd = new OracleCommand();
                if (mrno != null && mrno != "")
                {
                    cmd.CommandText = "select regno mrno,fname name,p.PHONE2 PHNO,'0' ipno from OTS1.PATIENTSREGISTRATION p where p.regno='" + mrno + "'";
                }
                else {
                    cmd.CommandText = "select i.ipno,to_char(casesheetnumber) mrno,i.name,i.PHNO2 PHNO from ip.ipinfo i  where i.ipno = '" + ipno + "'"; 
                }
                cmd.Connection = con;
                //int noOfRowDeleted = myapp.Database.ExecuteSqlCommand("update tbl_Patient set Bedallocated=0");
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        patient.mrno = dr["mrno"] != null ? dr["mrno"].ToString() : "";
                        patient.ipno = dr["ipno"] != null ? dr["ipno"].ToString() : "";
                        patient.PHNO = dr["PHNO"] != null ? dr["PHNO"].ToString() : "";
                     
                        patient.name = dr["name"] != null ? dr["name"].ToString() : "";
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Error " + ex.Message;
            }
            finally
            {
                con.Close();
            }
            return patient;
        }
    }
}
