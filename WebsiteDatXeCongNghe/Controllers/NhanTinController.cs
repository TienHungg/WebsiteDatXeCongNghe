using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteDatXeCongNghe.Models;

namespace WebsiteDatXeCongNghe.Controllers
{
    public class NhanTinController : Controller
    {
        private DichVuDatXeWebsite3Entities db = new DichVuDatXeWebsite3Entities();
        // GET: NhanTin
        //public JsonResult NoiDungNhanTin(string SDTNguoiNhan, string SDTNguoiGui)
        //{
        //    var NoiDung = db.NhanTins.Where(c => c.SDTNguoiNhan == SDTNguoiNhan && c.SDTNguoiGui == SDTNguoiGui || c.SDTNguoiNhan == SDTNguoiGui && c.SDTNguoiGui == SDTNguoiNhan).ToList();
        //    return Json(NoiDung, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult NoiDungNhanTin(string phoneNumber)
        //{
        //    var messages = db.NhanTins.Where(c => c.SDTNguoiNhan == phoneNumber || c.SDTNguoiGui == phoneNumber)
        //                          .OrderBy(c => c.NgayGui)
        //                          .ToList();
        //    return Json(messages, JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        public ActionResult NoiDungNhanTin(string phoneOfReceiver, string phoneOfSender)
        {
            var messages = db.NhanTins
                .Where(m => (m.SDTNguoiGui == phoneOfSender && m.SDTNguoiNhan == phoneOfReceiver) || (m.SDTNguoiGui == phoneOfReceiver && m.SDTNguoiNhan == phoneOfSender))
                .OrderBy(m => m.MaNhanTin) // sort messages in ascending order by MaNhanTin
                .Select(m => new
                {
                    Sender = m.SDTNguoiGui,
                    Receiver = m.SDTNguoiNhan,
                    Content = m.NoiDung
                })
                .ToList();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }



        //[HttpPost]
        //public ActionResult ThemNhanTin()
        //{
        //    return View();
        //}
        [HttpPost]
        public ActionResult ThemNhanTin(string message, string phoneOfSender, string phoneOfReceiver)
        {
            NhanTin nt = new NhanTin();
            nt.NoiDung = message;
            nt.SDTNguoiGui = phoneOfSender;
            nt.SDTNguoiNhan = phoneOfReceiver;
            db.NhanTins.Add(nt);
            db.SaveChanges();

            // Retrieve the newly added message with full details
            var addedMessage = db.NhanTins
                .Where(m => m.SDTNguoiGui == phoneOfSender && m.SDTNguoiNhan == phoneOfReceiver)
                .OrderByDescending(m => m.MaNhanTin)
                .Select(m => new
                {
                    Sender = m.SDTNguoiGui,
                    Receiver = m.SDTNguoiNhan,
                    Content = m.NoiDung
                })
                .FirstOrDefault();

            return Json(addedMessage, JsonRequestBehavior.AllowGet);
        }




    }
}