using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class AssetController : Controller
    {
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Asset
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NewAssetType(int id)
        {
            ViewBag.AssetTypeId = id;
            return View();
        }
        public ActionResult AssetTypeMaster()
        {
            return View();
        }
        public ActionResult AssetMaster()
        {
            return View();
        }
        public ActionResult NewAssetMaster()
        {
            return View();
        }
        public ActionResult CreateAsset(int id)
        {
            ViewBag.Assetid = id;
            return View();
        }
        public ActionResult UserAssetMaster()
        {

            return View();
        }
        public ActionResult MyAssets()
        {
            return View();
        }
        public ActionResult AssetTransferRequests()
        {
            var user = myapp.tbl_User.Where(m => m.CustomUserId == User.Identity.Name).FirstOrDefault();
            ViewBag.EmpNo = user.CustomUserId;
            ViewBag.Location = user.LocationId;
            ViewBag.Department = user.DepartmentId;
            return View();
        }
        public ActionResult ViewAsset(int id = 0)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult BarcodeRead(int id)
        {
            var Assetmodel = myapp.tbl_Asset.Where(m => m.AssetId == id).FirstOrDefault();
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(Assetmodel.SerialNumber + " " + Assetmodel.Name,
            QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            ViewBag.label = "*" + Assetmodel.SerialNumber + " " + Assetmodel.Name + "*";
            return View(BitmapToBytes(qrCodeImage));
        }
        public string PrintLabel(int id)
        {
            //Print();
            var Assetmodel = myapp.tbl_Asset.Where(m => m.AssetId == id).FirstOrDefault();

            string zpltext = "^XA";   //Start ZPL              
            zpltext += "^FO50,60^A0,30^FD" + Assetmodel.Name + "^FS";
            zpltext += "^FO120,100^BY3^BCN,70,,,,A^FD" + Assetmodel.AssetId + "^FS";
            zpltext += "^FO25,25^GB380,200,2^FS";
            zpltext += "^XZ";

            return zpltext;
        }

        //public ActionResult PrintLabel(int id)
        //{
        //    //Print();
        //    var Assetmodel = myapp.tbl_Asset.Where(m => m.AssetId == id).FirstOrDefault();
        //    StringBuilder ZplBuilder = new StringBuilder();
        //    string defaultPrinterName = string.Empty;
        //    PrinterSettings settings = new PrinterSettings();
        //    //Dictionary<string, string> oValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);


        //    ZplBuilder.Append("^XA");   //Start ZPL              
        //    ZplBuilder.Append("^FO50,60^A0,40^FD" + Assetmodel.Name + "^FS");
        //    ZplBuilder.Append("^FO60,120^BY3^BCN,60,,,,A^FD" + Assetmodel.AssetId + "^FS");
        //    ZplBuilder.Append("^FO25,25^GB380,200,2^FS");
        //    ZplBuilder.Append("^XZ");

        //    string ZplString = ZplBuilder.ToString();
        //    MemoryStream lmemStream = new MemoryStream();
        //    StreamWriter lstreamWriter = new StreamWriter(lmemStream);
        //    lstreamWriter.Write(ZplString);
        //    lstreamWriter.Flush();
        //    lmemStream.Position = 0;
        //    byte[] byteArray = lmemStream.ToArray();
        //    IntPtr cpUnmanagedBytes = new IntPtr(0);
        //    int cnLength = byteArray.Length;
        //    cpUnmanagedBytes = Marshal.AllocCoTaskMem(cnLength);
        //    Marshal.Copy(byteArray, 0, cpUnmanagedBytes, cnLength);
        //    System.Drawing.Printing.PrintDocument printDocument = new System.Drawing.Printing.PrintDocument();
        //    defaultPrinterName = printDocument.PrinterSettings.PrinterName;
        //    UpcLabel upcLabel = new UpcLabel();
        //    var msg = upcLabel.PrintBarcode("ZDesigner ZD230-203dpi ZPL", "hhh", ZplString.ToString(), "1");


        //    return Json(defaultPrinterName + "_" + msg, JsonRequestBehavior.AllowGet);
        //}
        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        public ActionResult ViewAssetPartial(int id = 0)
        {
            var Assetmodel = myapp.tbl_Asset.Where(m => m.AssetId == id).FirstOrDefault();
            AssetModel Asset = new AssetModel();
            Asset.AssetTypeId = Assetmodel.AssetTypeId;
            Asset.AssetTypeName = myapp.tbl_AssetType.Where(m => m.AssetTypeId == Asset.AssetTypeId).Select(n => n.Name).FirstOrDefault();
            Asset.Description = Assetmodel.Description;
            Asset.Impact = Assetmodel.Impact;
            Asset.LicenceTrackerId = Assetmodel.LicenceTrackerId;
            Asset.Manufacturer = Assetmodel.Manufacturer;
            Asset.ModeOfProcurement = Assetmodel.ModeOfProcurement;
            Asset.Name = Assetmodel.Name;
            Asset.Price = Assetmodel.Price;
            Asset.Quantity = Assetmodel.Quantity;
            Asset.Remarks = Assetmodel.Remarks;
            Asset.SerialNumber = Assetmodel.SerialNumber;
            Asset.Status = Assetmodel.Status;
            Asset.VendorId = Assetmodel.VendorId;
            Asset.VendorName = myapp.tbl_Vendor.Where(m => m.VendorId == Asset.VendorId).Select(n => n.Name).FirstOrDefault();
            Asset.Warranty = Assetmodel.Warranty;
            Asset.PurchaseDate = ProjectConvert.ConverDateTimeToString(Assetmodel.PurchaseDate.Value);
            Asset.CreatedBy = User.Identity.Name;
            Asset.CreatedOn = DateTime.Now;
            Asset.Insurance = Assetmodel.Insurance;
            Asset.Model = Assetmodel.Model;
            Asset.ServiceSupportDetails = Assetmodel.ServiceSupportDetails;
            Asset.RegistrationNo = Assetmodel.RegistrationNo;
            Asset.IsActive = true;
            return PartialView(Asset);
        }

        public ActionResult ViewUserAssetPartial(int id = 0)
        {
            var model = myapp.tbl_UserAssets.Where(m => m.UserAssetId == id).FirstOrDefault();
            UserAssetViewModel mode = new UserAssetViewModel();
            mode.UserId = model.UserId;
            mode.AssetId = model.AssetId;

            if (model.DateOfAssign != null)
            {
                mode.DateOfAssign = ProjectConvert.ConverDateTimeToString(model.DateOfAssign.Value);
            }
            mode.AllocationRequestId = model.AllocationRequestId;
            mode.AllocationStatus = model.AllocationStatus;
            mode.DeAllocateStatus = mode.AllocationStatus;

            mode.Remarks = model.Remarks;
            mode.IsActive = true;
            mode.CreatedOn = DateTime.Now;
            mode.CreatedBy = model.CreatedBy;
            var userdetails = myapp.tbl_User.Where(l => l.CustomUserId == model.CreatedBy).SingleOrDefault();
            var Assetmodel = (from var in myapp.tbl_Asset where var.AssetId == mode.AssetId select var).SingleOrDefault();
            AssetModel Asset = new AssetModel();
            mode.AssetSerialNumber = Assetmodel.SerialNumber;
            Asset.AssetTypeId = Assetmodel.AssetTypeId;
            Asset.AssetTypeName = myapp.tbl_AssetType.Where(m => m.AssetTypeId == Asset.AssetTypeId).Select(n => n.Name).FirstOrDefault();
            Asset.Description = Assetmodel.Description;
            Asset.Impact = Assetmodel.Impact;
            Asset.LicenceTrackerId = Assetmodel.LicenceTrackerId;
            Asset.Manufacturer = Assetmodel.Manufacturer;
            Asset.ModeOfProcurement = Assetmodel.ModeOfProcurement;
            Asset.Name = Assetmodel.Name;
            Asset.Price = Assetmodel.Price;
            Asset.Quantity = Assetmodel.Quantity;
            Asset.Remarks = Assetmodel.Remarks;
            Asset.SerialNumber = Assetmodel.SerialNumber;
            Asset.Status = Assetmodel.Status;
            Asset.VendorId = Assetmodel.VendorId;
            Asset.VendorName = myapp.tbl_Vendor.Where(m => m.VendorId == Asset.VendorId).Select(n => n.Name).FirstOrDefault();
            Asset.Warranty = Assetmodel.Warranty;
            Asset.PurchaseDate = ProjectConvert.ConverDateTimeToString(Assetmodel.PurchaseDate.Value);
            Asset.CreatedBy = User.Identity.Name;
            Asset.CreatedOn = DateTime.Now;
            Asset.Insurance = Assetmodel.Insurance;
            Asset.Model = Assetmodel.Model;
            Asset.ServiceSupportDetails = Assetmodel.ServiceSupportDetails;
            Asset.RegistrationNo = Assetmodel.RegistrationNo;
            Asset.IsActive = true;
            mode.AssetModel = Asset;
            mode.Department = userdetails.DepartmentName;
            mode.Designation = userdetails.Designation;
            return PartialView(mode);
        }
        public ActionResult GetAssets()
        {
            List<tbl_AssetType> list = myapp.tbl_AssetType.ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBrandsById(int id)
        {
            var list = myapp.tbl_AssetBrand.Where(m => m.AssetBrandId == id).FirstOrDefault();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetModelsById(int id)
        {
            var list = myapp.tbl_AssetModel.Where(m => m.AssetModelId == id).FirstOrDefault();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAssetListToAllocateUserbyassettype(int id)
        {
            var result = from s in myapp.tbl_Asset
                         where s.AssetTypeId == id && !myapp.tbl_UserAssets.Any(es => (es.AssetId == s.AssetId) && (es.AllocationStatus == "YES"))
                         && s.Status != "Scrap"
                         select new
                         {
                             s.AssetId,
                             s.Name,
                             s.SerialNumber,
                             s.Status

                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAssetListbyassettype(int id)
        {
            var result = from s in myapp.tbl_Asset

                         where s.AssetTypeId == id
                         select new
                         {
                             s.AssetId,
                             s.Name

                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAssetListbyUser(int userId)
        {
            var user = myapp.tbl_User.Where(m => m.UserId == userId).FirstOrDefault();
            if (user != null)
            {
                var userassets = (from u in myapp.tbl_UserAssets where u.UserId == user.CustomUserId select u).ToList();
                var assets = (from s in myapp.tbl_Asset select s).ToList();
                var result = (from s in assets
                              join a in userassets on s.AssetId equals a.AssetId

                              let DateOfAssign = a.DateOfAssign.Value
                              select new UserAssetViewModel
                              {
                                  AssetId = s.AssetId,
                                  AssetName = s.Name,
                                  AllocationStatus = a.AllocationStatus,
                                  AssetSerialNumber = a.AssetSerialNumber,
                                  DateOfAssign = DateOfAssign.ToString("dd/MM/yyyy")

                              }).ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAssetListbyLoginUser()
        {
            var id = User.Identity.Name;
            var user = myapp.tbl_User.Where(m => m.CustomUserId == id).FirstOrDefault();
            if (user != null)
            {
                var result = from s in myapp.tbl_Asset
                             join a in myapp.tbl_UserAssets on s.AssetId equals a.AssetId
                             where a.UserId == id
                             select new
                             {
                                 s.AssetId,
                                 s.Name

                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetAssetTypesandtypecomponents(int id)
        {
            List<tbl_AssetType> AssetType = myapp.tbl_AssetType.Where(l => l.AssetTypeId == id).ToList();
            List<tbl_AssetTypeComponent> Components = myapp.tbl_AssetTypeComponent.Where(k => k.AssetTypeId == id).ToList();

            var list = new { AssetType = AssetType, Components = Components };
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetAssetbyAssetType(int id)
        {

            List<tbl_Asset> Assetlist = myapp.tbl_Asset.Where(l => l.AssetTypeId == id).ToList();
            IEnumerable<AssetModel> Asset = (from var in Assetlist
                                             where var.AssetTypeId == id
                                             select new AssetModel
                                             {
                                                 AssetId = var.AssetId,
                                                 Name = var.Name,
                                                 Vendor = (from V in myapp.tbl_Vendor where V.VendorId == var.VendorId select V.Name).SingleOrDefault(),
                                                 Price = var.Price,
                                                 Quantity = var.Quantity,
                                                 Warranty = (var.Warranty != null ? var.Warranty : ""),
                                                 PurchaseDate = var.PurchaseDate.HasValue ? var.PurchaseDate.Value.ToString("MM-dd-yyyy") : "",
                                                 SerialNumber = (var.SerialNumber != null ? var.SerialNumber : "")

                                             });
            return Json(Asset, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetAssignUserbyAssetType(int id)
        {

            List<UserAssetViewModel> query = (from d in myapp.tbl_UserAssets
                                              join usr in myapp.tbl_User on d.UserId equals usr.CustomUserId
                                              join ass in myapp.tbl_Asset on d.AssetId equals ass.AssetId
                                              where ass.AssetTypeId == id
                                              select new UserAssetViewModel
                                              {

                                                  AssetName = ass.Name,
                                                  UserAssetId = d.UserAssetId,
                                                  AssetId = ass.AssetId,
                                                  UserId = usr.CustomUserId,
                                                  DTDateOfAssign = d.DateOfAssign,
                                                  DepartmentId = usr.DepartmentId.HasValue ? usr.DepartmentId.Value : 0,
                                                  UserName = usr.FirstName,
                                                  Department = usr.DepartmentName,
                                                  LocationId = usr.LocationId.HasValue ? usr.LocationId.Value : 0,
                                                  Location = usr.LocationName,
                                                  Remarks = d.Remarks,
                                                  DTDeAllocateDate = d.DeAllocateDate,
                                                  DeAllocateComments = d.DeAllocateComments != "" ? d.DeAllocateComments : null,
                                                  AllocationRequestId = d.AllocationRequestId,
                                                  AllocationStatus = d.AllocationStatus,
                                                  AssetSerialNumber = d.AssetSerialNumber


                                              }).ToList();
            var result = from c in query

                         select new
                         {
                             Location = c.Location,
                             Department = c.Department,
                             UserId = c.UserId,
                             UserName = c.UserName,
                             AssetName = c.AssetName,
                             DTDateOfAssign = c.DTDateOfAssign.HasValue ? c.DTDateOfAssign.Value.ToString("dd-MM-yyyy") : "",
                             AllocationRequestId = c.AllocationRequestId,
                             AllocationStatus = c.AllocationStatus,
                             Remarks = c.Remarks,
                             IsActive = c.IsActive.HasValue ? c.IsActive.ToString() : "false",
                             DTDeAllocateDate = c.DTDeAllocateDate.HasValue ? c.DTDeAllocateDate.Value.ToString("dd-MM-yyyy") : "",
                             DeAllocateComments = c.DeAllocateComments
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetAssetandcomponents(int id)
        {
            List<tbl_Asset> Asset = myapp.tbl_Asset.Where(l => l.AssetId == id).ToList();
            List<tbl_AssetComponent> Components = myapp.tbl_AssetComponent.Where(k => k.AssetId == id).ToList();

            var AssetTypeComponents = myapp.tbl_AssetComponent.Join(myapp.tbl_AssetTypeComponent,
                                 post => post.AssetTypeComponentId,
                                 meta => meta.AssetTypeComponentId,
                                 (post, meta) => new { Post = post, Meta = meta }).Where(k => k.Post.AssetId == id);
            string purchaseDate = Asset[0].PurchaseDate.HasValue ? Asset[0].PurchaseDate.Value.ToString("MM-dd-yyyy") : "";
            var list = new { Asset = Asset, Components = Components, purchaseDate = purchaseDate, AssetTypeComponents = AssetTypeComponents };
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //Get All assettypes
        public ActionResult GetAssetType()
        {
            List<tbl_AssetType> Asset = myapp.tbl_AssetType.ToList();
            return Json(Asset, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAssetTypeById(int id)
        {
            tbl_AssetType Asset = myapp.tbl_AssetType.Where(l => l.AssetTypeId == id).SingleOrDefault();
            return Json(Asset, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetComponentsbyassettype(int id)
        {
            List<tbl_AssetTypeComponent> Components = myapp.tbl_AssetTypeComponent.Where(k => k.AssetTypeId == id).ToList();
            return Json(Components, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSubAssets(int Id)
        {
            IQueryable<tbl_AssetType> list = myapp.tbl_AssetType.Where(l => l.AssetTypeId == Id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetAssetMasterDetails(JQueryDataTableParamModel param)
        {

            List<tbl_AssetType> query = (from d in myapp.tbl_AssetType select d).ToList();
            IEnumerable<tbl_AssetType> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.AssetTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Name != null && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Description != null && c.Description.ToString().ToLower().Contains(param.sSearch.ToLower())


                               ||
                               c.ShortCode != null && c.ShortCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_AssetType> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           select new object[] {


                                              c.AssetTypeId.ToString(),
                                             "",
                                              c.Name,

                                              c.Description,
                                              c.ShortCode,
                                              "",
                                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.AssetTypeId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetAssetTypeDetails(JQueryDataTableParamModel param)
        {

            List<tbl_AssetType> query = (from d in myapp.tbl_AssetType select d).ToList();
            IEnumerable<tbl_AssetType> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.AssetTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Name != null && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Description != null && c.Description.ToString().ToLower().Contains(param.sSearch.ToLower())


                               ||
                               c.ShortCode != null && c.ShortCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_AssetType> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           select new object[] {


                                              c.AssetTypeId.ToString(),
                                              c.Name,

                                              c.Description,
                                              c.ShortCode,
                                              //"",
                                              // myapp.tbl_Asset.Where(l=>l.AssetTypeId==c.AssetTypeId).Count(),
                                          //(from var in myapp.tbl_Asset join UA in myapp.tbl_UserAssets on var.AssetId equals UA.AssetId where var.AssetTypeId==c.AssetTypeId && UA.AllocationStatus=="YES" select UA).Count()    ,
                                           //((myapp.tbl_Asset.Where(l=>l.AssetTypeId==c.AssetTypeId).Count())-(from var in myapp.tbl_Asset join UA in myapp.tbl_UserAssets on var.AssetId equals UA.AssetId where var.AssetTypeId==c.AssetTypeId && UA.AllocationStatus=="YES" select UA).Count()),
            //c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.AssetTypeId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveNewAsset(tbl_AssetType model)
        {
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            myapp.tbl_AssetType.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAssetDeatilsById(int AssetId)
        {
            tbl_AssetType model = myapp.tbl_AssetType.Where(X => X.AssetTypeId == AssetId).SingleOrDefault();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateAsset(tbl_AssetType model)
        {
            List<tbl_AssetType> cat = myapp.tbl_AssetType.Where(l => l.AssetTypeId == model.AssetTypeId).ToList();
            if (cat.Count > 0)
            {
                cat[0].Name = model.Name;
                cat[0].Description = model.Description;
                //cat[0].Type = model.Type;
                cat[0].ShortCode = model.ShortCode;
                cat[0].IsActive = true;
                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;
                myapp.SaveChanges();
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveAssetandComponents(AssetModel Assetmodel, HttpPostedFileBase UploadRefferenceDocument1, HttpPostedFileBase UploadRefferenceDocument2, HttpPostedFileBase UploadRefferenceDocument3)
        {
            int Id = 0;
            if (Assetmodel.AssetId == 0)
            {
                tbl_Asset Asset = new tbl_Asset();
                if (UploadRefferenceDocument1 != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(UploadRefferenceDocument1.FileName);
                    string extension = Path.GetExtension(UploadRefferenceDocument1.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;
                    Asset.RefferenceDocument1 = fileName;
                    UploadRefferenceDocument1.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));

                }
                if (UploadRefferenceDocument2 != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(UploadRefferenceDocument2.FileName);
                    string extension = Path.GetExtension(UploadRefferenceDocument2.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;
                    Asset.RefferenceDocument2 = fileName;
                    UploadRefferenceDocument2.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));

                }
                if (UploadRefferenceDocument3 != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(UploadRefferenceDocument3.FileName);
                    string extension = Path.GetExtension(UploadRefferenceDocument3.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;
                    Asset.RefferenceDocument3 = fileName;
                    UploadRefferenceDocument3.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));

                }
                Asset.PONumber = Assetmodel.PONumber;
                Asset.AssetTypeId = Assetmodel.AssetTypeId;
                Asset.Description = Assetmodel.Description;
                Asset.Impact = Assetmodel.Impact;
                Asset.LicenceTrackerId = Assetmodel.LicenceTrackerId;
                Asset.Manufacturer = Assetmodel.Manufacturer;
                Asset.ModeOfProcurement = Assetmodel.ModeOfProcurement;
                Asset.Name = Assetmodel.Name;
                Asset.Price = Assetmodel.Price;
                Asset.Quantity = Assetmodel.Quantity;
                Asset.Remarks = Assetmodel.Remarks;
                Asset.SerialNumber = Assetmodel.SerialNumber;
                Asset.Status = Assetmodel.Status;
                Asset.VendorId = Assetmodel.VendorId;
                Asset.Warranty = Assetmodel.Warranty;
                Asset.PurchaseDate = ProjectConvert.ConverDateStringtoDatetime(Assetmodel.PurchaseDate);
                Asset.CreatedBy = User.Identity.Name;
                Asset.CreatedOn = DateTime.Now;
                Asset.Insurance = Assetmodel.Insurance;
                Asset.Model = Assetmodel.Model;
                Asset.ServiceSupportDetails = Assetmodel.ServiceSupportDetails;
                Asset.RegistrationNo = Assetmodel.RegistrationNo;
                Asset.IsActive = true;
                Asset.BrandId = Assetmodel.BrandId;
                myapp.tbl_Asset.Add(Asset);
                myapp.SaveChanges();
                Id = Asset.AssetId;
            }
            else
            {
                tbl_Asset cat = myapp.tbl_Asset.Where(l => l.AssetId == Assetmodel.AssetId).SingleOrDefault();
                if (UploadRefferenceDocument1 != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(UploadRefferenceDocument1.FileName);
                    string extension = Path.GetExtension(UploadRefferenceDocument1.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;
                    if (cat.RefferenceDocument1 != null)
                    {
                        string filePath = Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), cat.RefferenceDocument1);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    cat.RefferenceDocument1 = fileName;
                    UploadRefferenceDocument1.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));

                }
                if (UploadRefferenceDocument2 != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(UploadRefferenceDocument2.FileName);
                    string extension = Path.GetExtension(UploadRefferenceDocument2.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;
                    if (cat.RefferenceDocument2 != null)
                    {
                        string filePath = Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), cat.RefferenceDocument2);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    cat.RefferenceDocument2 = fileName;
                    UploadRefferenceDocument2.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));

                }
                if (UploadRefferenceDocument3 != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(UploadRefferenceDocument2.FileName);
                    string extension = Path.GetExtension(UploadRefferenceDocument2.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;
                    if (cat.RefferenceDocument3 != null)
                    {
                        string filePath = Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), cat.RefferenceDocument3);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    cat.RefferenceDocument3 = fileName;
                    UploadRefferenceDocument2.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));

                }

                cat.AssetTypeId = Assetmodel.AssetTypeId;
                cat.Description = Assetmodel.Description;
                cat.Impact = Assetmodel.Impact;
                cat.LicenceTrackerId = Assetmodel.LicenceTrackerId;
                cat.Manufacturer = Assetmodel.Manufacturer;
                cat.ModeOfProcurement = Assetmodel.ModeOfProcurement;
                cat.Name = Assetmodel.Name;
                cat.Price = Assetmodel.Price;
                cat.Quantity = Assetmodel.Quantity;
                cat.Remarks = Assetmodel.Remarks;
                cat.SerialNumber = Assetmodel.SerialNumber;
                cat.Status = Assetmodel.Status;
                cat.VendorId = Assetmodel.VendorId;
                cat.Warranty = Assetmodel.Warranty;
                cat.PurchaseDate = ProjectConvert.ConverDateStringtoDatetime(Assetmodel.PurchaseDate);
                cat.ModifiedBy = User.Identity.Name;
                cat.ModifiedOn = DateTime.Now;
                cat.Insurance = Assetmodel.Insurance;
                cat.Model = Assetmodel.Model;
                cat.BrandId = Assetmodel.BrandId;
                cat.ServiceSupportDetails = Assetmodel.ServiceSupportDetails;
                cat.RegistrationNo = Assetmodel.RegistrationNo;
                cat.IsActive = true;
                myapp.SaveChanges();
                Id = cat.AssetId;
            }
            string json = Id.ToString();

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveAssetType(tbl_AssetType Asset)
        {
            if (Asset.AssetTypeId == 0)
            {
                Asset.CreatedBy = User.Identity.Name;
                Asset.CreatedOn = DateTime.Now;
                myapp.tbl_AssetType.Add(Asset);
            }
            else
            {
                tbl_AssetType cat = myapp.tbl_AssetType.Where(l => l.AssetTypeId == Asset.AssetTypeId).SingleOrDefault();
                cat.Name = Asset.Name;
                cat.Description = Asset.Description;
                cat.ShortCode = Asset.ShortCode;
                cat.ModifiedBy = User.Identity.Name;
                cat.ModifiedOn = DateTime.Now;
                cat.IsActive = Asset.IsActive;
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveAssetTypeandComponentsTypes(tbl_AssetType Asset, tbl_AssetTypeComponent[] ComponentsList)
        {
            if (Asset.AssetTypeId == 0)
            {
                Asset.CreatedBy = User.Identity.Name;
                Asset.CreatedOn = DateTime.Now;
                myapp.tbl_AssetType.Add(Asset);
            }
            else
            {
                tbl_AssetType cat = myapp.tbl_AssetType.Where(l => l.AssetTypeId == Asset.AssetTypeId).SingleOrDefault();
                cat.Name = Asset.Name;
                cat.Description = Asset.Description;
                cat.ShortCode = Asset.ShortCode;
                cat.ModifiedBy = User.Identity.Name;
                cat.ModifiedOn = DateTime.Now;
                cat.IsActive = Asset.IsActive;
            }
            myapp.SaveChanges();

            if (ComponentsList != null)
            {
                for (int i = 0; i < ComponentsList.Length; i++)
                {
                    if (ComponentsList[i].AssetTypeComponentId == 0)
                    {
                        tbl_AssetTypeComponent com = new tbl_AssetTypeComponent
                        {
                            AssetTypeId = Asset.AssetTypeId,
                            Description = ComponentsList[i].Description,
                            IsActive = true,
                            Name = ComponentsList[i].Name,
                            ShortCode = ComponentsList[i].ShortCode,
                            CreatedBy = User.Identity.Name,
                            CreatedOn = DateTime.Now
                        };
                        myapp.tbl_AssetTypeComponent.Add(com);
                    }
                    else
                    {
                        int id = ComponentsList[i].AssetTypeComponentId;
                        tbl_AssetTypeComponent compon = myapp.tbl_AssetTypeComponent.Where(k => k.AssetTypeComponentId == id).SingleOrDefault();
                        if (ComponentsList[i].IsActive == false)
                        {
                            if (compon != null)
                            {
                                myapp.tbl_AssetTypeComponent.Remove(compon);

                            }
                        }
                        else
                        {
                            compon.AssetTypeId = Asset.AssetTypeId;
                            compon.Description = ComponentsList[i].Description;
                            compon.IsActive = true;
                            compon.Name = ComponentsList[i].Name;
                            compon.ShortCode = ComponentsList[i].ShortCode;
                            compon.ModifiedBy = User.Identity.Name;
                            compon.ModifiedOn = DateTime.Now;
                        }
                    }
                    myapp.SaveChanges();
                }
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveAssetComponents(List<tbl_AssetComponent> ComponentsList)
        {

            if (ComponentsList != null)
            {
                for (int i = 0; i < ComponentsList.Count; i++)
                {
                    if (ComponentsList[i].AssetComponentId == 0)
                    {
                        tbl_AssetComponent com = new tbl_AssetComponent
                        {
                            AssetId = ComponentsList[0].AssetId,
                            AssetTypeComponentId = ComponentsList[0].AssetTypeComponentId,
                            AssetTypeId = ComponentsList[0].AssetTypeId,
                            Manufacturer = ComponentsList[i].Manufacturer,
                            IsActive = true,
                            Model = ComponentsList[i].Model,
                            Price = ComponentsList[i].Price,
                            Quantity = ComponentsList[i].Quantity,
                            Warranty = ComponentsList[i].Warranty,
                            Remarks = ComponentsList[i].Remarks,
                            Impact = ComponentsList[i].Impact,
                            SerialNumber = ComponentsList[i].SerialNumber
                        };
                        myapp.tbl_AssetComponent.Add(com);
                    }
                    else
                    {
                        int id = ComponentsList[i].AssetComponentId;
                        tbl_AssetComponent compon = myapp.tbl_AssetComponent.Where(k => k.AssetComponentId == id).SingleOrDefault();
                        if (ComponentsList[i].IsActive == false)
                        {
                            if (compon != null)
                            {
                                myapp.tbl_AssetComponent.Remove(compon);

                            }
                        }
                        else
                        {
                            compon.AssetId = ComponentsList[0].AssetId;
                            compon.AssetTypeComponentId = ComponentsList[i].AssetTypeComponentId;
                            compon.AssetTypeId = ComponentsList[0].AssetTypeId;
                            compon.Manufacturer = ComponentsList[i].Manufacturer;
                            compon.IsActive = true;
                            compon.Model = ComponentsList[i].Model;
                            compon.Price = ComponentsList[i].Price;
                            compon.Quantity = ComponentsList[i].Quantity;
                            compon.Warranty = ComponentsList[i].Warranty;
                            compon.Remarks = ComponentsList[i].Remarks;
                            compon.Impact = ComponentsList[i].Impact;
                            compon.SerialNumber = ComponentsList[i].SerialNumber;
                        }
                    }
                    myapp.SaveChanges();
                }
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetAssetDetails(JQueryDataTableParamModel param)
        {

            List<tbl_Asset> query = (from d in myapp.tbl_Asset select d).OrderByDescending(l => l.AssetId).ToList();
            if (param.AssetTypeId != 0)
            {
                query = query.Where(l => l.AssetTypeId == param.AssetTypeId).ToList(); ;
            }
            if (param.status != null && param.status != "")
            {
                query = query.Where(l => l.Status == param.status).ToList();

            }
            if (param.todate != null && param.todate != "")
            {
                query = query.Where(l => l.PurchaseDate == ProjectConvert.ConverDateStringtoDatetime(param.todate)).ToList();
            }
            IEnumerable<tbl_Asset> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.AssetId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Name != null && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Description != null && c.Description.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Status != null && c.Status.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.SerialNumber != null && c.SerialNumber.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Remarks != null && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.Manufacturer != null && c.Manufacturer.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;

            }
            IEnumerable<tbl_Asset> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = (from c in displayedCompanies
                                            join a in myapp.tbl_AssetType on c.AssetTypeId equals a.AssetTypeId
                                            join v in myapp.tbl_Vendor on c.VendorId equals v.VendorId
                                            select new object[] {
                                                c.AssetId,
                                                a.Name.ToString(),
                                                c.Name,
                                                c.Description,
                                                v.Name,
                                                c.Price,
                                                c.Quantity,
                                                c.Warranty,
                                                c.PurchaseDate.HasValue ? c.PurchaseDate.Value.ToString("dd-MM-yyyy") : "",
                                                c.SerialNumber,
                                                c.Status,
                                                c.Status=="Scrap"?"0": c.AssetId.ToString()
                         }).Distinct();
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAssetType(int AssetTypeId)
        {
            int AssetCount = myapp.tbl_Asset.Where(a => a.AssetTypeId == AssetTypeId).Count();
            tbl_AssetType v = myapp.tbl_AssetType.Where(a => a.AssetTypeId == AssetTypeId).SingleOrDefault();

            if (v != null)
            {
                if (AssetCount == 0)
                {

                    myapp.tbl_AssetType.Remove(v);
                    myapp.SaveChanges();
                }
                else
                {
                    v.IsActive = false;
                }
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        //#endregion
        //#region UserAssetMaster
        public ActionResult AjaxGetUserAssetDetails(JQueryDataTableParamModel param)
        {
            var usrassts = myapp.tbl_UserAssets.ToList();
            var assts = myapp.tbl_Asset.ToList();
            List<UserAssetViewModel> query = (from d in usrassts
                                                  //join usr in myapp.tbl_User on d.UserId equals usr.CustomUserId
                                              join ass in assts on d.AssetId equals ass.AssetId
                                              let usr = d.UserId != null && d.UserId != "0" ? myapp.tbl_User.Where(l => l.CustomUserId == d.UserId).FirstOrDefault() : new tbl_User()
                                              select new UserAssetViewModel
                                              {
                                                  AssetName = ass.Name,
                                                  UserAssetId = d.UserAssetId,
                                                  AssetId = ass.AssetId,
                                                  UserId = d.UserId,
                                                  UserName = (usr != null && usr.FirstName != null) ? usr.FirstName : "",
                                                  DTDateOfAssign = d.DateOfAssign,
                                                  LocationId = d.LocationId.HasValue ? d.LocationId.Value : 0,
                                                  DepartmentId = d.DepartmentId.HasValue ? d.DepartmentId.Value : 0,
                                                  Remarks = d.Remarks,
                                                  DTDeAllocateDate = d.DeAllocateDate,
                                                  DeAllocateComments = d.DeAllocateComments != "" ? d.DeAllocateComments : null,
                                                  AllocationRequestId = d.AllocationRequestId,
                                                  AllocationStatus = d.AllocationStatus,
                                                  AssetSerialNumber = d.AssetSerialNumber,
                                                  AssetAllocationType = d.AssetAllocationType,
                                                  FloorId = d.FloorId,
                                                  RoomId = d.RoomId,
                                                  BuildingId = d.BuildingId,
                                                  FloorName = d.FloorId != null && d.FloorId > 0 ? (myapp.tbl_Floor.Where(l => l.FloorId == d.FloorId).SingleOrDefault().FloorName) : "",
                                                  BuildingName = d.BuildingId != null && d.BuildingId > 0 ? (myapp.tbl_Building.Where(l => l.BuildingId == d.BuildingId).SingleOrDefault().BuildingName) : "",
                                                  RoomName = d.RoomId != null && d.RoomId > 0 ? (myapp.tbl_Room.Where(l => l.RoomId == d.RoomId).SingleOrDefault().RoomName) : "",
                                              }).ToList();
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            if (param.locationid != null && param.locationid != 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.departmentid != null && param.departmentid != 0)
            {
                query = query.Where(l => l.DepartmentId == param.departmentid).ToList();
            }

            if (param.Emp != null && param.Emp != "")
            {
                query = query.Where(l => l.UserId == param.Emp).ToList();
            }
            IEnumerable<UserAssetViewModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.UserAssetId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.AssetName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.DateOfAssign != null && c.DateOfAssign.Contains(param.sSearch.ToLower())
                              ||
                              c.UserId != null && c.UserId.Contains(param.sSearch.ToLower())
                               ||
                              c.UserName != null && c.UserName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.AssetSerialNumber != null && c.AssetSerialNumber.ToLower().Contains(param.sSearch.ToLower())

                              ||
                               c.Remarks != null && c.Remarks.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<UserAssetViewModel> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies

                                           select new object[] {
                                               c.AssetAllocationType,
                             (from d in myapp.tbl_Location where d.LocationId == c.LocationId select d.LocationName).FirstOrDefault(),
                             (from d in myapp.tbl_Department where d.DepartmentId == c.DepartmentId select d.DepartmentName).FirstOrDefault(),
                            c.UserId,
                            c.FloorName!=null && c.FloorName!=""?( c.BuildingName+"-"+c.FloorName+"-"+c.RoomName +"- Incharge is "+c.UserName):c.UserName,
                            c.AssetName,
                            c.DTDateOfAssign.HasValue ? c.DTDateOfAssign.Value.ToString("dd-MM-yyyy") : "",
                            c.AllocationRequestId,
                            c.AllocationStatus,
                            c.Remarks,
                           // c.AssetId,
                           //c.AllocationStatus=="YES"?"Allocated":"Deallocated",
                          c.DTDeAllocateDate.HasValue ? c.DTDeAllocateDate.Value.ToString("dd-MM-yyyy") : "",
                            c.DeAllocateComments,
                            c.UserAssetId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetMyAssetDetails(JQueryDataTableParamModel param)
        {
            List<UserAssetViewModel> query = (from d in myapp.tbl_UserAssets
                                              join usr in myapp.tbl_User on d.UserId equals usr.CustomUserId

                                              join ass in myapp.tbl_Asset on d.AssetId equals ass.AssetId

                                              select new UserAssetViewModel
                                              {

                                                  AssetName = ass.Name,
                                                  UserAssetId = d.UserAssetId,
                                                  AssetId = ass.AssetId,
                                                  UserId = usr.CustomUserId,
                                                  DTDateOfAssign = d.DateOfAssign,
                                                  DepartmentId = usr.DepartmentId.HasValue ? usr.DepartmentId.Value : 0,
                                                  UserName = usr.FirstName,
                                                  Department = usr.DepartmentName,
                                                  LocationId = usr.LocationId.HasValue ? usr.LocationId.Value : 0,
                                                  Location = usr.LocationName,
                                                  Remarks = d.Remarks,
                                                  DTDeAllocateDate = d.DeAllocateDate,
                                                  DeAllocateComments = d.DeAllocateComments != "" ? d.DeAllocateComments : null,
                                                  AllocationRequestId = d.AllocationRequestId,
                                                  AllocationStatus = d.AllocationStatus,
                                                  AssetSerialNumber = d.AssetSerialNumber


                                              }).ToList();
            query = query.Where(m => m.UserId == User.Identity.Name).ToList();
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            if (param.locationid != null && param.locationid != 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.departmentid != null && param.departmentid != 0)
            {
                query = query.Where(l => l.DepartmentId == param.departmentid).ToList();
            }

            if (param.Emp != null && param.Emp != "")
            {
                query = query.Where(l => l.UserId == param.Emp).ToList();
            }



            IEnumerable<UserAssetViewModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.UserAssetId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.AssetName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.DateOfAssign != null && c.DateOfAssign.ToString().ToLower().Contains(param.sSearch.ToLower())

                              ||
                               c.Remarks != null && c.Remarks.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<UserAssetViewModel> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies

                                           select new object[] {
                            c.Location,
                            c.Department,
                            c.UserId,
                            c.UserName,
                            c.AssetName,
                            c.DTDateOfAssign.HasValue ? c.DTDateOfAssign.Value.ToString("dd-MM-yyyy") : "",
                            c.AllocationRequestId,
                            c.AllocationStatus,
                            c.Remarks,
                           // c.AssetId,
                           //c.AllocationStatus=="YES"?"Allocated":"Deallocated",
                          c.DTDeAllocateDate.HasValue ? c.DTDeAllocateDate.Value.ToString("dd-MM-yyyy") : "",
                            c.DeAllocateComments,
                            c.UserAssetId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserAssetDetails(int id)
        {
            var usrast = myapp.tbl_UserAssets.Where(l => l.UserAssetId == id).SingleOrDefault();
            if (usrast != null)
            {
                UserAssetViewModel objm = new UserAssetViewModel();
                objm.AllocationStatus = usrast.AllocationStatus;
                objm.AllocationRequestId = usrast.AllocationRequestId;
                objm.AssetAllocationType = usrast.AssetAllocationType;
                objm.AssetId = usrast.AssetId;
                if (objm.AssetId != null && objm.AssetId > 0)
                {
                    var dbmodel = (from var in myapp.tbl_Asset where var.AssetId == objm.AssetId select var).SingleOrDefault();
                    objm.AssetName = dbmodel.Name;
                    //objm.AssetModel = dbmodel.Model;
                }
                objm.AssetSerialNumber = usrast.AssetSerialNumber;
                objm.BuildingId = usrast.BuildingId;
                if (objm.BuildingId != null && objm.BuildingId > 0)
                {
                    var dbm = (from a in myapp.tbl_Building where a.BuildingId == objm.BuildingId select a).SingleOrDefault();
                    objm.BuildingName = dbm.BuildingName;
                }
                objm.CreatedBy = usrast.CreatedBy;
                objm.CreatedOn = usrast.CreatedOn;
                objm.DateOfAssign = usrast.DateOfAssign.HasValue ? usrast.DateOfAssign.Value.ToString("dd/MM/yyyy") : "";
                objm.DeAllocateComments = usrast.DeAllocateComments;
                objm.DeAllocateDate = usrast.DeAllocateDate.HasValue ? usrast.DeAllocateDate.Value.ToString("dd/MM/yyyy") : "";
                objm.DeAllocateStatus = usrast.DeAllocateStatus;
                objm.DeAllocationRequestId = usrast.DeAllocationRequestId;
                objm.DepartmentId = usrast.DepartmentId.HasValue ? usrast.DepartmentId.Value : 0;
                if (objm.DepartmentId != null && objm.DepartmentId > 0)
                {
                    var dbm = (from a in myapp.tbl_Department where a.DepartmentId == objm.DepartmentId select a).SingleOrDefault();
                    objm.Department = dbm.DepartmentName;
                }
                objm.Designation = "";
                objm.FloorId = usrast.FloorId;
                if (objm.FloorId != null && objm.FloorId > 0)
                {
                    var dbm = (from a in myapp.tbl_Floor where a.FloorId == objm.FloorId select a).SingleOrDefault();
                    objm.FloorName = dbm.FloorName;
                }
                objm.IsActive = usrast.IsActive;
                objm.LocationId = usrast.LocationId.HasValue ? usrast.LocationId.Value : 0;
                if (objm.LocationId != null && objm.LocationId > 0)
                {
                    var dbm = (from a in myapp.tbl_Location where a.LocationId == objm.LocationId select a).SingleOrDefault();
                    objm.Location = dbm.LocationName;
                }
                objm.ModifiedBy = User.Identity.Name;
                objm.ModifiedOn = usrast.ModifiedOn;
                objm.Remarks = usrast.Remarks;
                objm.RoomId = usrast.RoomId;
                if (objm.RoomId != null && objm.RoomId > 0)
                {
                    var dbm = (from a in myapp.tbl_Room where a.RoomId == objm.RoomId select a).SingleOrDefault();
                    objm.RoomName = dbm.RoomName;
                }
                objm.UserAssetId = usrast.UserAssetId;
                objm.UserId = usrast.UserId;
                if (objm.UserId != null && objm.UserId != "" && objm.UserId != "0")
                {
                    var dbm = (from a in myapp.tbl_User where a.CustomUserId == objm.UserId select a).SingleOrDefault();
                    objm.UserName = dbm.FirstName;
                }
                return Json(objm, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveNewUserAsset(UserAssetViewModel model)
        {
            tbl_UserAssets mode = new tbl_UserAssets();
            mode.UserId = model.UserId;
            mode.AssetId = model.AssetId;
            var dbmodel = (from var in myapp.tbl_Asset where var.AssetId == mode.AssetId select var).SingleOrDefault();
            if (model.DateOfAssign != null)
            {
                mode.DateOfAssign = ProjectConvert.ConverDateStringtoDatetime(model.DateOfAssign);
            }
            var userdetails = myapp.tbl_User.Where(l => l.CustomUserId == model.UserId).SingleOrDefault();
            mode.LocationId = model.LocationId;
            mode.DepartmentId = model.DepartmentId;
            if (model.AssetAllocationType != null && model.AssetAllocationType == "To User")
            {
                mode.LocationId = userdetails.LocationId;
                mode.DepartmentId = userdetails.DepartmentId;
                mode.BuildingId = 0;
                mode.FloorId = 0;
                mode.RoomId = 0;
            }
            else
            {
                mode.LocationId = model.LocationId;
                mode.DepartmentId = model.DepartmentId;
                mode.BuildingId = model.BuildingId;
                mode.FloorId = model.FloorId;
                mode.RoomId = model.RoomId;
            }
            mode.AssetAllocationType = model.AssetAllocationType;
            mode.AllocationRequestId = model.AllocationRequestId;
            mode.AllocationStatus = model.AllocationStatus;
            mode.DeAllocateStatus = (mode.AllocationStatus == "YES") ? "NO" : "YES";
            mode.AssetSerialNumber = dbmodel.SerialNumber;
            mode.Remarks = model.Remarks;
            mode.IsActive = true;
            mode.CreatedOn = DateTime.Now;
            mode.CreatedBy = User.Identity.Name;
            myapp.tbl_UserAssets.Add(mode);
            myapp.SaveChanges();
            dbmodel.Status = "Allocated";
            myapp.SaveChanges();
            //Asset History:
            tbl_AssetHistory history = new tbl_AssetHistory();
            history.AssetId = model.AssetId;
            history.IsActive = true;
            history.LogDate = DateTime.Now;
            history.LogDescription = "Asset : " + dbmodel.Name + " has allocated to " + userdetails.FirstName;
            history.LogSubject = "Asset : " + dbmodel.Name + " has allocated to " + userdetails.FirstName;
            myapp.tbl_AssetHistory.Add(history);
            myapp.SaveChanges();
            // SendEmailUserAsset();
            return Json(mode.UserAssetId, JsonRequestBehavior.AllowGet);
        }
        public void SendEmailUserAsset(string body, int id)
        {
            var userAsset = myapp.tbl_UserAssets.Where(m => m.UserAssetId == id).FirstOrDefault();
            var email = myapp.tbl_User.Where(m => m.CustomUserId == userAsset.CreatedBy).Select(n => n.EmailId).FirstOrDefault();
            if (email != null && email != "")
            {
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel
                {
                    fromemail = "Leave@hospitals.com",
                    // toemail = email,
                    toemail = "phanisrinivas111@gmail.com",
                    //  subject = Subject,
                    body = body,
                    filepath = "",
                    fromname = "",
                    ccemail = "",
                    subject = "Assert is Assigned"
                };
                cm.SendEmail(mailmodel);
            }

        }
        public JsonResult GetUserAssetDeatilsById(int UserAssetId)
        {
            tbl_UserAssets query = myapp.tbl_UserAssets.Where(X => X.UserAssetId == UserAssetId).SingleOrDefault();
            string UserName = (from var in myapp.tbl_User where var.CustomUserId == query.UserId select var.FirstName).FirstOrDefault();
            string Asset = (from var in myapp.tbl_Asset where var.AssetId == query.AssetId select var.Name).FirstOrDefault();
            string startDate = query.DateOfAssign.HasValue ? query.DateOfAssign.Value.ToString("MM-dd-yyyy") : "";
            var obj = new { query, startDate, UserName, Asset };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateUserAsset(UserAssetViewModel model)
        {
            List<tbl_UserAssets> cat = myapp.tbl_UserAssets.Where(l => l.UserAssetId == model.UserAssetId).ToList();
            if (cat.Count > 0)
            {
                if (model.DateOfAssign != null)
                {
                    cat[0].DateOfAssign = ProjectConvert.ConverDateStringtoDatetime(model.DateOfAssign);
                }
                cat[0].AllocationRequestId = model.AllocationRequestId;
                cat[0].AllocationStatus = model.AllocationStatus;
                cat[0].DeAllocateStatus = (cat[0].AllocationStatus == "YES") ? "NO" : "YES";
                cat[0].Remarks = model.Remarks;
                cat[0].IsActive = true;
                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;
                myapp.SaveChanges();
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateToScrapAsset(int assetId, string comments)
        {
            var dbmodel = (from var in myapp.tbl_Asset where var.AssetId == assetId select var).SingleOrDefault();
            dbmodel.Remarks = comments;
            dbmodel.Status = "Scrap";
            myapp.SaveChanges();
            tbl_AssetHistory history = new tbl_AssetHistory();
            history.AssetId = assetId;
            history.IsActive = true;
            history.LogDate = DateTime.Now;
            string UserId = User.Identity.Name;
            var userdetails = myapp.tbl_User.Where(l => l.CustomUserId == UserId).SingleOrDefault();
            history.LogDescription = "Asset : " + dbmodel.Name + " has Scraped by " + userdetails.FirstName + " Due to " + comments;
            history.LogSubject = "Asset : " + dbmodel.Name + "  has Scraped by " + userdetails.FirstName + " Due to " + comments;
            myapp.tbl_AssetHistory.Add(history);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeallocateAsset(UserAssetViewModel model)
        {
            tbl_UserAssets u = new tbl_UserAssets
            {
                UserAssetId = model.UserAssetId,
                DeAllocateDate = ProjectConvert.ConverDateStringtoDatetime(model.DeAllocateDate),
                DeAllocateComments = model.DeAllocateComments,
                DeAllocationRequestId = model.DeAllocationRequestId,
                DeAllocateStatus = model.DeAllocateStatus,
                AllocationStatus = (model.DeAllocateStatus == "YES") ? "NO" : "YES"
            };
            List<tbl_UserAssets> cat = myapp.tbl_UserAssets.Where(l => l.UserAssetId == model.UserAssetId).ToList();
            if (cat.Count > 0)
            {
                cat[0].DeAllocateComments = u.DeAllocateComments;
                cat[0].DeAllocateDate = u.DeAllocateDate;
                cat[0].DeAllocateStatus = u.DeAllocateStatus;
                cat[0].AllocationStatus = u.AllocationStatus;
                cat[0].DeAllocationRequestId = u.DeAllocationRequestId;
                myapp.SaveChanges();
                int AssetId = cat[0].AssetId.Value;
                var dbmodel = (from var in myapp.tbl_Asset where var.AssetId == AssetId select var).SingleOrDefault();
                dbmodel.Status = "Deallocated";
                myapp.SaveChanges();
                string UserIdcustom = cat[0].UserId;
                tbl_AssetHistory history = new tbl_AssetHistory();
                history.AssetId = AssetId;
                history.IsActive = true;
                history.LogDate = DateTime.Now;
                var userdetails = myapp.tbl_User.Where(l => l.CustomUserId == UserIdcustom).SingleOrDefault();
                history.LogDescription = "Asset : " + dbmodel.Name + " has Deallocated from " + userdetails.FirstName;
                history.LogSubject = "Asset : " + dbmodel.Name + " has Deallocated from " + userdetails.FirstName;
                myapp.tbl_AssetHistory.Add(history);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteUserAsset(int UserAssetId)
        {
            tbl_UserAssets v = myapp.tbl_UserAssets.Where(a => a.UserAssetId == UserAssetId).FirstOrDefault();
            if (v != null)
            {
                v.IsActive = false;
                //myapp.tbl_UserAssets.Remove(v);
                myapp.SaveChanges();
                int AssetId = v.AssetId.Value;
                var dbmodel = (from var in myapp.tbl_Asset where var.AssetId == AssetId select var).SingleOrDefault();
                tbl_AssetHistory history = new tbl_AssetHistory();
                history.AssetId = v.AssetId;
                history.IsActive = true;
                history.LogDate = DateTime.Now;
                var userdetails = myapp.tbl_User.Where(l => l.CustomUserId == v.UserId).SingleOrDefault();
                history.LogDescription = "Asset : " + dbmodel.Name + " has In Activated";
                history.LogSubject = "Asset : " + dbmodel.Name + " has In Activated";
                myapp.tbl_AssetHistory.Add(history);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAssetSerialNumber(int assettypeid)
        {
            var assets = myapp.tbl_Asset.Where(l => l.AssetTypeId == assettypeid).OrderByDescending(l => l.AssetId).ToList();
            var assetype = myapp.tbl_AssetType.Where(l => l.AssetTypeId == assettypeid).SingleOrDefault();
            if (assets.Count > 0)
            {
                string assetnumber = "FH/CO/" + assetype.Name + "/";
                try
                {
                    int number = int.Parse(assets[0].Name.Split('/')[3]);
                    number = number + 1;
                    return Json("FH/CO/" + assetype.Name + "/" + number.ToString("X5"), JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json("FH/CO/" + assetype.Name + "/00001", JsonRequestBehavior.AllowGet);
                };
            }
            else
            {
                return Json("FH/CO/" + assetype.Name + "/00001", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetAssetHistory(int assetId)
        {
            var dbmodel = (from var in myapp.tbl_AssetHistory where var.AssetId == assetId select var).ToList();
            var details = (from d in dbmodel
                           let DateOfLog = d.LogDate != null ? d.LogDate.Value.ToString("dd/MM/yyyy") : ""
                           select new
                           {
                               d.AssetId,
                               DateOfLog,
                               d.LogSubject,
                               d.LogDescription
                           }).ToList();
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AddAssetHistory(int assetId, string comments)
        {
            tbl_AssetHistory m = new tbl_AssetHistory();
            m.AssetId = assetId;
            m.IsActive = true;
            m.LogDate = DateTime.Now;
            m.LogDescription = comments;
            m.LogSubject = comments;
            myapp.tbl_AssetHistory.Add(m);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);

        }
        public JsonResult ExportExcelUserAsset(string FromDate, string ToDate)
        {
            DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(FromDate);
            DateTime todate = ProjectConvert.ConverDateStringtoDatetime(ToDate);
            List<tbl_UserAssets> Result = myapp.tbl_UserAssets.Where(p => p.DateOfAssign >= fromdate && p.DateOfAssign <= todate).ToList();
            GridView grid = new GridView
            {
                DataSource = Result
            };
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=UserAsset.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveDocuments()
        {
            int UserAssetId = Convert.ToInt32(Request.Form["UserAssetId"]);
            string Remarks = Request.Form["Remarks"].ToString();

            if (Request.Files.Count > 0)
            {
                try
                {

                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname;
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        string fileName = Path.GetFileName(file.FileName);
                        string guidid = Guid.NewGuid().ToString();
                        string path = Path.Combine(Server.MapPath("~/Documents/"), guidid + fileName);
                        file.SaveAs(path);

                        tbl_UserAssertDocuments doc = new tbl_UserAssertDocuments
                        {
                            UserAssetId = UserAssetId,
                            DocumentUrl = guidid + fileName,
                            Remarks = Remarks,
                            Createdon = DateTime.Now
                        };
                        myapp.tbl_UserAssertDocuments.Add(doc);
                        myapp.SaveChanges();


                    }

                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
        [HttpPost]
        public JsonResult GetFiles(int AId)
        {
            var files = from file in myapp.tbl_UserAssertDocuments
                        where file.UserAssetId == AId
                        select new { Did = file.DocumentId, Remarks = file.Remarks, fileName = file.DocumentUrl };
            return Json(files, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult DeleteDocument(int fileId)
        {

            tbl_UserAssertDocuments v = myapp.tbl_UserAssertDocuments.Where(a => a.DocumentId == fileId).FirstOrDefault();
            if (v != null)
            {
                myapp.tbl_UserAssertDocuments.Remove(v);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageBrand()
        {
            return View();
        }
        public ActionResult SaveBrand(tbl_AssetBrand model)
        {
            var dbmodel = new tbl_AssetBrand();
            if (model.AssetBrandId != 0)
            {
                dbmodel = myapp.tbl_AssetBrand.Where(m => m.AssetBrandId == model.AssetBrandId).SingleOrDefault();

            }
            dbmodel.ModifiedBy = User.Identity.Name;
            dbmodel.ModifiedOn = DateTime.Now;
            dbmodel.AssetTypeId = model.AssetTypeId;
            dbmodel.Name = model.Name;
            dbmodel.Description = model.Description;


            dbmodel.IsActive = true;
            if (model.AssetBrandId == 0)
            {
                dbmodel.CreatedBy = User.Identity.Name;
                dbmodel.CreatedOn = DateTime.Now;
                myapp.tbl_AssetBrand.Add(model);
            }

            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAssetBrand(int id)
        {
            var brnds = myapp.tbl_AssetBrand.Where(l => l.AssetBrandId == id).ToList();
            if (brnds.Count > 0)
            {
                myapp.tbl_AssetBrand.Remove(brnds[0]);
                myapp.SaveChanges();
            }
            return Json("Delete Success");
        }
        public ActionResult DeleteAssets(int id)
        {
            var brnds = myapp.tbl_Asset.Where(l => l.AssetId == id).ToList();
            if (brnds.Count > 0)
            {
                myapp.tbl_Asset.Remove(brnds[0]);
                myapp.SaveChanges();
            }
            return Json("Delete Success");
        }


        public ActionResult GetAssetBrands()
        {
            List<tbl_AssetBrand> query = (from d in myapp.tbl_AssetBrand select d).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAssetBrandsByType(int id)
        {
            List<tbl_AssetBrand> query = myapp.tbl_AssetBrand.Where(l => l.AssetTypeId == id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetAssetBrandsDetails(JQueryDataTableParamModel param)
        {
            IQueryable<tbl_AssetType> assettypemaster = myapp.tbl_AssetType.AsQueryable();
            List<tbl_AssetBrand> query = (from d in myapp.tbl_AssetBrand select d).ToList();
            IEnumerable<tbl_AssetBrand> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.AssetTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Name != null && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Description != null && c.Description.ToString().ToLower().Contains(param.sSearch.ToLower())


                               ||
                               c.ShortCode != null && c.ShortCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_AssetBrand> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           join a in assettypemaster on c.AssetTypeId equals a.AssetTypeId
                                           select new object[] {
                                              c.AssetBrandId.ToString(),
                                              a.Name,// Asset type name
                                              c.Name,
                                              c.Description,
                                              c.ShortCode,
                                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.AssetBrandId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageModel()
        {
            return View();
        }



        public ActionResult SaveAssetModel(tbl_AssetModel model)
        {
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            model.IsActive = true;
            myapp.tbl_AssetModel.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAssetModel(int id)
        {
            var brnds = myapp.tbl_AssetModel.Where(l => l.AssetModelId == id).ToList();
            if (brnds.Count > 0)
            {
                myapp.tbl_AssetModel.Remove(brnds[0]);
                myapp.SaveChanges();
            }
            return Json("Delete Success");
        }
        public ActionResult ChangeStatus(string status, int id)
        {
            var brnds = myapp.tbl_Asset.Where(l => l.AssetId == id).FirstOrDefault();
            brnds.Status = status;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAssetModels()
        {
            List<tbl_AssetModel> query = (from d in myapp.tbl_AssetModel select d).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAssetModelsByType(int id)
        {
            List<tbl_AssetModel> query = myapp.tbl_AssetModel.Where(l => l.AssetBrandId == id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetAssetModelsDetails(JQueryDataTableParamModel param)
        {
            IQueryable<tbl_AssetType> assettypemaster = myapp.tbl_AssetType.AsQueryable();
            IQueryable<tbl_AssetBrand> assetbrandmaster = myapp.tbl_AssetBrand.AsQueryable();
            List<tbl_AssetModel> query = (from d in myapp.tbl_AssetModel select d).ToList();
            IEnumerable<tbl_AssetModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.AssetTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Name != null && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Desciption != null && c.Desciption.ToString().ToLower().Contains(param.sSearch.ToLower())


                               ||
                               c.ShortCode != null && c.ShortCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_AssetModel> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           join a in assettypemaster on c.AssetTypeId equals a.AssetTypeId
                                           join b in assetbrandmaster on c.AssetBrandId equals b.AssetBrandId
                                           select new object[] {
                                              c.AssetModelId.ToString(),
                                              a.Name,// Asset type name
                                              b.Name, //Asset Brand
                                              c.Name,
                                              c.Desciption,
                                              c.ShortCode,
                                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.AssetModelId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AjaxGetAssetTransferDetails(JQueryDataTableParamModel param)
        {

            List<tbl_AssetTranfer> query = (from d in myapp.tbl_AssetTranfer select d).ToList();
            if (param.typeofitem == "User")
            {
                query = query.Where(m => m.CreatedBy == User.Identity.Name).ToList();
            }
            IEnumerable<tbl_AssetTranfer> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.RequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.RequestUserId != null && c.RequestUserId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.DateOfRequest != null && c.DateOfRequest.ToString().ToLower().Contains(param.sSearch.ToLower())


                               ||
                               c.CurrentLocationId != null && c.CurrentLocationId.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_AssetTranfer> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           select new object[] {
                                              c.RequestId.ToString(),
                                              (from d in myapp.tbl_Location where d.LocationId == c.CurrentLocationId select d.LocationName).FirstOrDefault(),
                                         (from d in myapp.tbl_Location where d.LocationId == c.ToLocationId select d.LocationName).FirstOrDefault(),
                                              (from d in myapp.tbl_Department where d.DepartmentId == c.CurrentDepartmentId select d.DepartmentName).FirstOrDefault(),
                                         (from d in myapp.tbl_Department where d.DepartmentId == c.ToDepartmentId select d.DepartmentName).FirstOrDefault(),
                                            (from d in myapp.tbl_User where d.EmpId == c.RequestUserId select d.FirstName).FirstOrDefault(),
                                           (from d in myapp.tbl_User where d.EmpId == c.ToUserId select d.FirstName).FirstOrDefault(),
                                           c.DateOfRequest!=null?c.DateOfRequest.Value.ToString("dd/MM/yyyy"):"",
                                              c.ReasonToTranfer,
                                              c.Remarks,
                                              c.RequestId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveNewUserAssetTransfer(tbl_AssetTranfer model)
        {
            var dbmodel = (from var in myapp.tbl_UserAssets where var.UserAssetId == model.UserAssetId select var).SingleOrDefault();
            var asset = myapp.tbl_Asset.Where(m => m.AssetId == dbmodel.AssetId).FirstOrDefault();
            var requestedUser = myapp.tbl_User.Where(m => m.CustomUserId == User.Identity.Name).Select(m => m.UserId).FirstOrDefault();
            model.AssetId = asset.AssetId;
            model.TranferStatus = "Pending";
            model.RequestUserId = requestedUser;
            model.AssetName = asset.Name;
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            model.CurrentDepartmentId = dbmodel.DepartmentId;
            model.CurrentLocationId = dbmodel.LocationId;
            model.CreatedBy = User.Identity.Name;
            model.DateOfRequest = DateTime.Now;
            model.FromBuildingId = dbmodel.BuildingId;
            model.FromFloorId = dbmodel.FloorId;
            model.FromRoomId = dbmodel.RoomId;
            model.CreatedOn = DateTime.Now;
            myapp.tbl_AssetTranfer.Add(model);
            myapp.SaveChanges();
            // SendEmailUserAsset();
            return Json(model.UserAssetId, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveNewUserAssetTransferAsset(tbl_AssetTranfer model)
        {
            //var asset = myapp.tbl_Asset.Where(m => m.AssetId == model.AssetId).FirstOrDefault();
            //var dbmodel = (from var in myapp.tbl_UserAssets where var.AssetId == asset.AssetId select var).SingleOrDefault();
            //var requestedUser = myapp.tbl_User.Where(m => m.CustomUserId == User.Identity.Name).Select(m => m.UserId).FirstOrDefault();
            if (model.RequestId != 0)
            {
                var editModel = myapp.tbl_AssetTranfer.Where(n => n.RequestId == model.RequestId).FirstOrDefault();
                editModel.AssetId = 0;
                editModel.UserAssetId = 0;
                editModel.TranferStatus = "Pending";
                editModel.RequestUserId = model.RequestUserId;
                editModel.AssetName = model.AssetName;
                editModel.CreatedBy = User.Identity.Name;
                editModel.CreatedOn = DateTime.Now;
                editModel.CurrentDepartmentId = model.CurrentDepartmentId;
                editModel.CurrentLocationId = model.CurrentLocationId;
                editModel.ReasonToTranfer = model.ReasonToTranfer;
                editModel.Remarks = model.Remarks;
                editModel.ToDepartmentId = model.ToDepartmentId;
                editModel.ToLocationId = model.ToLocationId;
                editModel.ToUserId = model.ToUserId;
                editModel.ModifiedBy = User.Identity.Name;
                editModel.ModifiedOn = DateTime.Now;
                model.IsHODApproved = true;
                myapp.SaveChanges();
            }
            else
            {
                model.UserAssetId = 0;
                model.AssetId = 0;

                model.CreatedBy = User.Identity.Name;
                model.CreatedOn = DateTime.Now;
                model.DateOfRequest = DateTime.Now;
                model.IsHODApproved = false;
                myapp.tbl_AssetTranfer.Add(model);
                myapp.SaveChanges();
                SendEmailtoHOD(model);
            }
            // SendEmailUserAsset();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult Approval_Reject_AssetTransfer(int id, string status, string comments)
        {
            tbl_AssetTranfer request = new tbl_AssetTranfer();
            request = myapp.tbl_AssetTranfer.Where(m => m.RequestId == id).FirstOrDefault();
            request.IsHODApproved = true;
            request.HODComments = comments;
            if (status == "Reject")
            {
                request.TranferStatus = "HOD Reject";
            }
            else
            {
                request.TranferStatus = "HOD Approve";
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public void SendEmailtoHOD(tbl_AssetTranfer model)
        {
            string body = string.Empty;

            var user = myapp.tbl_User.Where(m => m.EmpId == model.RequestUserId).FirstOrDefault();
            var hod = myapp.tbl_User.Where(m => m.UserId == user.ReportingManagerId).FirstOrDefault();
            if (user != null && hod != null)
            {
                var mealTable = "<table style='border:1px solid #eee;width: 60%;'>";
                mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>Tranfer Id</td><td style='border:1px solid #eee;font-family:Segoe UI'>" + model.RequestId + "</td></tr>";
                mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>Request Employee Name</td><td style='border:1px solid #eee;font-family:Segoe UI'>" + myapp.tbl_User.Where(n => n.EmpId == model.RequestUserId).Select(n => n.FirstName).FirstOrDefault() + "</td></tr>";
                mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>From Location</td><td style='border:1px solid #eee;font-family:Segoe UI'>" + myapp.tbl_Location.Where(n => n.LocationId == model.CurrentLocationId).Select(n => n.LocationName).FirstOrDefault() + "</td></tr>";
                mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>From Department</td><td style='border:1px solid #eee;font-family:Segoe UI'>" + myapp.tbl_Department.Where(n => n.DepartmentId == model.CurrentDepartmentId).Select(n => n.DepartmentName).FirstOrDefault() + "</td></tr>";
                mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>To Employee Name</td><td style='border:1px solid #eee;font-family:Segoe UI'>" + myapp.tbl_User.Where(n => n.EmpId == model.ToUserId).Select(n => n.FirstName).FirstOrDefault() + "</td></tr>";
                mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>To Location</td><td style='border:1px solid #eee;font-family:Segoe UI'>" + myapp.tbl_Location.Where(n => n.LocationId == model.ToLocationId).Select(n => n.LocationName).FirstOrDefault() + "</td></tr>";
                mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>To Department</td><td style='border:1px solid #eee;font-family:Segoe UI'>" + myapp.tbl_Department.Where(n => n.DepartmentId == model.ToDepartmentId).Select(n => n.DepartmentName).FirstOrDefault() + "</td></tr>";
                mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>Date of Request</td><td style='border:1px solid #eee;font-family:Segoe UI'>" + model.DateOfRequest.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>Reason</td><td style='border:1px solid #eee;font-family:Segoe UI'>" + model.ReasonToTranfer + "</td></tr>";
                mealTable += "</table>";
                string Subject = "";
                body += "Dear " + hod.FirstName + " , <br/> Please find below are the Asset Transfer Request details waiting for your approval. ";
                body += mealTable;
                Subject = "Asset transfer Request from " + user.FirstName + "";
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel
                {
                    fromemail = "Leave@hospitals.com",
                    toemail = hod.EmailId,
                    subject = Subject,
                    body = body,
                    filepath = "",
                    fromname = "",
                    ccemail = ""
                };
                cm.SendEmail(mailmodel);
            }
        }
        public JsonResult GetAssetTransfers(int id)
        {
            var model = myapp.tbl_AssetTranfer.Where(m => m.RequestId == id).FirstOrDefault();

            var fromLocation = myapp.tbl_Location.Where(n => n.LocationId == model.CurrentLocationId).Select(m => m.LocationName).FirstOrDefault();
            var toLocation = myapp.tbl_Location.Where(n => n.LocationId == model.ToLocationId).Select(m => m.LocationName).FirstOrDefault();
            var fromdept = myapp.tbl_Department.Where(n => n.DepartmentId == model.CurrentDepartmentId).Select(m => m.DepartmentName).FirstOrDefault();
            var toDept = myapp.tbl_Department.Where(n => n.DepartmentId == model.ToDepartmentId).Select(m => m.DepartmentName).FirstOrDefault();
            var fromUser = myapp.tbl_User.Where(n => n.EmpId == model.RequestUserId).Select(m => m.FirstName).FirstOrDefault();
            var toUser = "";
            if (model.ToUserId != null && model.ToUserId != 0)
                toUser = myapp.tbl_User.Where(n => n.EmpId == model.ToUserId).Select(m => m.FirstName).FirstOrDefault();
            var obj = new { model = model, fromLocation = fromLocation, toLocation = toLocation, fromdept = fromdept, toDept = toDept, fromUser = fromUser, toUser = toUser };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConfirmAssetTransfer(int id, string remarks)
        {
            var model = myapp.tbl_AssetTranfer.Where(m => m.RequestId == id).FirstOrDefault();
            var userAsset = myapp.tbl_UserAssets.Where(m => m.UserAssetId == model.UserAssetId).FirstOrDefault();
            userAsset.LocationId = model.ToLocationId;
            userAsset.DepartmentId = model.ToDepartmentId;
            userAsset.ModifiedBy = User.Identity.Name;
            userAsset.ModifiedOn = DateTime.Now;
            myapp.SaveChanges();
            model.TranferStatus = "Completed";
            model.ModifiedOn = DateTime.Now;
            model.ModifiedBy = User.Identity.Name;
            model.Remarks = remarks;
            myapp.SaveChanges();
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult MobileAllotment()
        {
            return View();
        }
        public ActionResult NewMobileAllotment(int id = 0, int requestId = 0)
        {
            ViewBag.id = id;
            ViewBag.requestId = requestId;
            return View();
        }
        public JsonResult SaveMobileAllotment(AssetMobileAllotmentViewModel model)
        {
            tbl_Asset_MobileAllotment dbmodel = new tbl_Asset_MobileAllotment();
            if (model.Id != 0)
            {
                dbmodel = myapp.tbl_Asset_MobileAllotment.Where(m => m.Id == model.Id).SingleOrDefault();
                dbmodel.ModifiedBy = User.Identity.Name;
                dbmodel.ModifiedOn = DateTime.Now;
            }
            dbmodel.BrandId = model.BrandID;
            dbmodel.ModelId = model.ModelID;
            dbmodel.Subject = model.Subject;
            dbmodel.Description = model.Description;
            dbmodel.SerialNumber = model.SerialNumber;
            dbmodel.BatterySerialNo = model.BatterySerialNo;
            if (model.DOP != null && model.DOP != "")
                dbmodel.DOP = ProjectConvert.ConverDateStringtoDatetime(model.DOP);
            dbmodel.IMEINO = model.IMEINO;
            dbmodel.AssetLabel = model.AssetLabel;
            dbmodel.Accessories = model.Accessories;
            dbmodel.MonthlyPlan = model.MonthlyPlan;
            dbmodel.SIMNO = model.SIMNO;
            dbmodel.MobileNumber = model.MobileNumber;
            dbmodel.CellPhoneRequestId = model.CellPhoneRequestId;
            if (dbmodel.CellPhoneRequestId != null && dbmodel.CellPhoneRequestId != 0)
            {
                var cell = myapp.tbl_CellPhoneRequest.Where(m => m.CellPhoneRequestId == dbmodel.CellPhoneRequestId).FirstOrDefault();
                dbmodel.AssigntoEmpId = cell.EmpNo;
                cell.Status = "IT - Updated";
                cell.ModifiedBy = User.Identity.Name;
                cell.ModifiedOn = DateTime.Now;
                myapp.SaveChanges();
            }
            dbmodel.AssignDate = DateTime.Now;
            dbmodel.AssetStatus = model.AssetStatus;
            if (model.Id == 0)
            {
                dbmodel.CreatedBy = User.Identity.Name;
                dbmodel.CreatedOn = DateTime.Now;
                dbmodel.ModifiedBy = User.Identity.Name;
                dbmodel.ModifiedOn = DateTime.Now;
                myapp.tbl_Asset_MobileAllotment.Add(dbmodel);
            }
            myapp.SaveChanges();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMobileAllotment(int id)
        {
            var model = myapp.tbl_Asset_MobileAllotment.Where(l => l.Id == id).SingleOrDefault();
            AssetMobileAllotmentViewModel dbmodel = new AssetMobileAllotmentViewModel();
            var brand = myapp.tbl_AssetBrand.Where(l => l.AssetBrandId == model.BrandId).SingleOrDefault();
            dbmodel.AssetTypeId = brand.AssetTypeId;
            dbmodel.BrandID = model.BrandId;
            dbmodel.ModelID = model.ModelId;
            dbmodel.Subject = model.Subject;
            dbmodel.Description = model.Description;
            dbmodel.SerialNumber = model.SerialNumber;
            dbmodel.BatterySerialNo = model.BatterySerialNo;
            if (model.DOP != null)
                dbmodel.DOP = ProjectConvert.ConverDateTimeToString(model.DOP.Value);
            dbmodel.IMEINO = model.IMEINO;
            dbmodel.AssetLabel = model.AssetLabel;
            dbmodel.Accessories = model.Accessories;
            dbmodel.MonthlyPlan = model.MonthlyPlan;
            dbmodel.SIMNO = model.SIMNO;
            dbmodel.MobileNumber = model.MobileNumber;
            dbmodel.AssetStatus = model.AssetStatus;

            dbmodel.AssigntoEmpId = model.AssigntoEmpId;
            if (model.AssignDate != null)
                dbmodel.AssignDate = ProjectConvert.ConverDateTimeToString(model.AssignDate.Value);
            dbmodel.AssignComments = model.AssignComments;
            dbmodel.IssuedByEmpId = model.IssuedByEmpId;
            dbmodel.AssignEmpAchknowledged = model.AssignEmpAchknowledged;
            dbmodel.ReturnReceivedBy = model.ReturnReceivedBy;
            dbmodel.CellPhoneRequestId = model.CellPhoneRequestId;
            if (model.ReturnReceivedOn != null)
                dbmodel.ReturnReceivedOn = ProjectConvert.ConverDateTimeToString(model.ReturnReceivedOn.Value);
            dbmodel.ReturnReceivedComments = model.ReturnReceivedComments;
            return Json(dbmodel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetMobileAllotment(JQueryDataTableParamModel param)
        {
            List<tbl_Asset_MobileAllotment> query = (from d in myapp.tbl_Asset_MobileAllotment select d).ToList();
            query = query.OrderByDescending(m => m.Id).ToList();
            if (param.Brand != null && param.Brand != 0)
            {
                query = query.Where(m => m.BrandId == param.Brand).ToList();
            }
            if (param.Model != null && param.Model != 0)
            {
                query = query.Where(m => m.ModelId == param.Model).ToList();
            }
            if (param.status != null && param.status != "")
            {
                query = query.Where(m => m.AssetStatus == param.status).ToList();
            }
            if (param.Emp != null && param.Emp != "")
            {
                var userId = Convert.ToInt32(param.Emp);
                var id = myapp.tbl_User.Where(m => m.UserId == userId).Select(n => n.EmpId).FirstOrDefault();
                query = query.Where(m => m.AssigntoEmpId == id).ToList();
            }
            if (param.todate != null && param.todate != "")
            {
                query = query.Where(m => m.AssignDate.Value.ToString("dd/MM/yyyy") == param.todate).ToList();
            }
            IEnumerable<tbl_Asset_MobileAllotment> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.AssetLabel != null && c.AssetLabel.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.SerialNumber != null && c.SerialNumber.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.MobileNumber != null && c.MobileNumber.ToLower().Contains(param.sSearch.ToLower())
                               || c.AssetStatus != null && c.AssetStatus.ToLower().Contains(param.sSearch.ToLower())
                               || c.AssigntoEmpId != null && myapp.tbl_User.Where(l => l.EmpId == c.AssigntoEmpId).SingleOrDefault().FirstName.ToLower().Contains(param.sSearch.ToLower())
                                                              || c.BrandId != null && myapp.tbl_AssetBrand.Where(l => l.AssetBrandId == c.BrandId).SingleOrDefault().Name.ToLower().Contains(param.sSearch.ToLower())
                                                              || c.ModelId != null && myapp.tbl_AssetModel.Where(l => l.AssetModelId == c.ModelId).SingleOrDefault().Name.ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_Asset_MobileAllotment> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           let emp = myapp.tbl_User.Where(l => l.EmpId == c.AssigntoEmpId).SingleOrDefault()
                                           select new object[] {
                                               c.Id.ToString(),
                                               c.AssetLabel,
                                            myapp.tbl_AssetBrand.Where(n=>n.AssetBrandId==c.BrandId ).Select(n=>n.Name).FirstOrDefault(),
                                               myapp.tbl_AssetModel.Where(n=>n.AssetModelId==c.ModelId ).Select(n=>n.Name).FirstOrDefault(),
                                               c.SerialNumber,
                                               c.MobileNumber,
                                              emp!=null? emp.FirstName:"",
                                                  c.AssignDate.HasValue?c.AssignDate.Value.ToString("dd/MM/yyyy"):"",
                                               c.AssetStatus,

                                               c.Id.ToString()
                         };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AssignMobileToEmp(int id, int empid, string comments)
        {
            var tbluser = myapp.tbl_User.Where(l => l.UserId == empid).SingleOrDefault();
            var model = myapp.tbl_Asset_MobileAllotment.Where(l => l.Id == id).SingleOrDefault();
            model.AssigntoEmpId = tbluser.EmpId;
            model.AssignComments = comments;
            model.AssignDate = DateTime.Now;
            model.AssetStatus = "InUse";
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReturnMobileToEmp(int id, int empid, string comments)
        {
            var tbluser = myapp.tbl_User.Where(l => l.UserId == empid).SingleOrDefault();
            var model = myapp.tbl_Asset_MobileAllotment.Where(l => l.Id == id).SingleOrDefault();
            model.ReturnReceivedBy = tbluser.EmpId;
            model.ReturnReceivedComments = comments;
            model.ReturnReceivedOn = DateTime.Now;
            model.AssetStatus = "Returned";
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPendingMobileAllotment()
        {
            var query = myapp.tbl_Asset_MobileAllotment.Where(n => n.AssigntoEmpId == null || n.AssigntoEmpId == 0).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportExcelAssets(string assettype, string date, string status)
        {
            List<tbl_Asset> query = (from d in myapp.tbl_Asset select d).ToList();
            if (date != null && date != "" && date != null && date != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(date);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).ToList();
            }
            if (status != null && status != "")
            {
                query = query.Where(m => m.Status.ToLower() == status.ToLower()).ToList();
            }
            if (assettype != null && assettype != "")
            {
                int intassettype = int.Parse(assettype);
                query = query.Where(m => m.AssetTypeId == intassettype).ToList();
            }

            var products = new System.Data.DataTable("Assets");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Asset No", typeof(string));
            products.Columns.Add("Serial Number", typeof(string));
            products.Columns.Add("Description", typeof(string));
            products.Columns.Add("Status", typeof(string));
            products.Columns.Add("Price", typeof(string));
            products.Columns.Add("Purchase Date", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Service Support", typeof(string));
            products.Columns.Add("Model", typeof(string));
            products.Columns.Add("Brand", typeof(string));

            foreach (var item in query)
            {
                products.Rows.Add(
            item.AssetId,
            item.Name,
            item.SerialNumber,
            item.Description,
            item.Status,
            item.Price.HasValue ? item.Price.Value.ToString() : "0",
            item.PurchaseDate.HasValue ? item.PurchaseDate.Value.ToString("dd/MM/yyyy") : "",
            item.Remarks,
            item.ServiceSupportDetails,
            item.Model,
            item.BrandId.HasValue ? item.BrandId.Value.ToString() : "0"

                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=AssetsExport.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CreateQRCode()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateQRCode(QRCodeModel qRCode)
        {
            //var WebUri = new Url(qRCode.QRCodeText);
            //string UriPayload = WebUri;
            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(qRCode.QRCodeText, QRCodeGenerator.ECCLevel.Q);
            QRCode QrCode = new QRCode(QrCodeInfo);
            Bitmap QrBitmap = QrCode.GetGraphic(60);
            byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
            ViewBag.QrCodeUri = QrUri;
            return View();
        }
    }
    public static class BitmapExtension
    {
        public static byte[] BitmapToByteArray(this Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}