using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    public class DeviceInfoController : ApiController
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DeviceInfoController));
        private MyIntranetAppEntities db = new MyIntranetAppEntities();

        // GET: api/DeviceInfo
        public IQueryable<tbl_DeviceInfo> Gettbl_DeviceInfo()
        {
            return db.tbl_DeviceInfo;
        }

        // GET: api/DeviceInfo/5
        [ResponseType(typeof(tbl_DeviceInfo))]
        public IHttpActionResult Gettbl_DeviceInfo(int id)
        {
            tbl_DeviceInfo tbl_DeviceInfo = db.tbl_DeviceInfo.Find(id);
            if (tbl_DeviceInfo == null)
            {
                return NotFound();
            }

            return Ok(tbl_DeviceInfo);
        }

        // PUT: api/DeviceInfo/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Puttbl_DeviceInfo(int id, tbl_DeviceInfo tbl_DeviceInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tbl_DeviceInfo.Id)
            {
                return BadRequest();
            }

            db.Entry(tbl_DeviceInfo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tbl_DeviceInfoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/DeviceInfo
        [ResponseType(typeof(tbl_DeviceInfo))]
        public IHttpActionResult Posttbl_DeviceInfo(tbl_DeviceInfo tbl_DeviceInfo)
        {
            logger.Info("User Id" + tbl_DeviceInfo.UserId + " Device Id " + tbl_DeviceInfo.DeviceId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var list = db.tbl_DeviceInfo.Where(l => l.UserId == tbl_DeviceInfo.UserId).ToList();
            if (list.Count > 0)
            {
                list[0].DeviceId = tbl_DeviceInfo.DeviceId;
                list[0].IsActive = true;
                list[0].ModifiedOn = DateTime.Now;
                db.SaveChanges();
            }
            else
            {
                tbl_DeviceInfo.IsActive = true;
                tbl_DeviceInfo.ModifiedOn = DateTime.Now;
                tbl_DeviceInfo.CreatedOn = DateTime.Now;
                db.tbl_DeviceInfo.Add(tbl_DeviceInfo);
                db.SaveChanges();
            }

            return Ok(tbl_DeviceInfo);
        }

        // DELETE: api/DeviceInfo/5
        [ResponseType(typeof(tbl_DeviceInfo))]
        public IHttpActionResult Deletetbl_DeviceInfo(int id)
        {
            tbl_DeviceInfo tbl_DeviceInfo = db.tbl_DeviceInfo.Find(id);
            if (tbl_DeviceInfo == null)
            {
                return NotFound();
            }

            db.tbl_DeviceInfo.Remove(tbl_DeviceInfo);
            db.SaveChanges();

            return Ok(tbl_DeviceInfo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tbl_DeviceInfoExists(int id)
        {
            return db.tbl_DeviceInfo.Count(e => e.Id == id) > 0;
        }
    }
}