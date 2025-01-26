using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteDatXeCongNghe.Models;

namespace WebsiteDatXeCongNghe.Controllers
{
    public class DanhGiaUngDungController : Controller
    {
        private DichVuDatXeWebsite3Entities db = new DichVuDatXeWebsite3Entities();
        // GET: DanhGiaUngDung
        public ActionResult DanhGiaUngDung()
        {
            var list = db.DanhGiaUngDungs.Include("KhachHang").ToList();
            return View(list);
        }
        [HttpPost]
        public ActionResult ThemDanhGiaUngDung(string phone, DateTime date, int value_rate, string content)
        {
            DanhGiaUngDung dg = new DanhGiaUngDung();
            dg.SoDienThoai = phone;
            dg.NgayDanhGia = date;
            dg.MucDo = value_rate;
            dg.NoiDung = content;
            db.DanhGiaUngDungs.Add(dg);
            db.SaveChanges();

            // Load KhachHang entity for the new review
            dg = db.DanhGiaUngDungs.Include("KhachHang").FirstOrDefault(d => d.SoDienThoai == dg.SoDienThoai);

            var newReview = new
            {
                HinhAnh = dg.KhachHang != null ? dg.KhachHang.HinhAnh : null,
                NgayDanhGia = dg.NgayDanhGia.ToString("yyyy-MM-dd"),
                Ten = dg.KhachHang != null ? dg.KhachHang.Ten : null,
                NoiDung = dg.NoiDung,
                MucDo = dg.MucDo
            };

            return Json(newReview);
        }


        [HttpGet]
        public ActionResult XuatHienDanhGiaUngDung()
        {
            ViewBag.Model = db.DanhGiaUngDungs.Include("KhachHang").ToList();
            return View();
        }
        //[HttpPost]
        //public ActionResult SuaDanhGiaUngDung(string phoneNumber)
        //{
        //    return Json(new { success = true });
        //}
        //[HttpPost]
        //public ActionResult SuaDanhGiaUngDung(string phoneNumber, string noiDung)
        //{
        //    var existingReview = db.DanhGiaUngDungs.FirstOrDefault(d => d.SoDienThoai == phoneNumber);

        //    if (existingReview != null)
        //    {
        //        existingReview.NoiDung = noiDung;
        //        db.Entry(existingReview).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //    }

        //    bool hasReviewContent = !string.IsNullOrEmpty(existingReview?.NoiDung);

        //    var response = new
        //    {
        //        hasReviewContent,

        //        reviewContent = existingReview?.NoiDung

        //    };

        //    return Json(response);
        //}

        [HttpPost]
        public ActionResult SuaDanhGiaUngDung(string phoneNumber, string noiDung, int mucDo)
        {
            // Retrieve the existing review based on the phoneNumber
            var existingReview = db.DanhGiaUngDungs.FirstOrDefault(d => d.SoDienThoai == phoneNumber);

            if (existingReview != null)
            {
                existingReview.NoiDung = noiDung;
                existingReview.MucDo = mucDo;
                db.Entry(existingReview).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            // Create a response object with the updated review details
            var response = new
            {
                HinhAnh = existingReview.KhachHang.HinhAnh,
                NgayDanhGia = existingReview.NgayDanhGia.ToString("dd/MM/yyyy"),
                Ten = existingReview.KhachHang.Ten,
                NoiDung = existingReview.NoiDung,
                MucDo = existingReview.MucDo
            };

            return Json(response);
        }


        //[HttpPost]
        //public ActionResult SuaDanhGiaUngDung(string phoneNumber)
        //{
        //    var existingReview = db.DanhGiaUngDungs.FirstOrDefault(d => d.SoDienThoai == phoneNumber);

        //    if (existingReview != null)
        //    {
        //        return Json(existingReview.NoiDung);
        //    }

        //    return Json(null);
        //}

        public ActionResult CheckNoiDung(string phoneNumber)
        {
            var review = db.DanhGiaUngDungs.FirstOrDefault(d => d.SoDienThoai == phoneNumber);

            var response = new
            {
                hasNoiDung = (review != null && !string.IsNullOrEmpty(review.NoiDung)),
                noiDung = review?.NoiDung,
                mucDo = review?.MucDo
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult XoaDanhGiaUngDung(string phoneNumber)
        {
            var danhGia = db.DanhGiaUngDungs.FirstOrDefault(d => d.SoDienThoai == phoneNumber);
            if (danhGia != null)
            {
                db.DanhGiaUngDungs.Remove(danhGia);
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }


        //[HttpPost]
        //public ActionResult SuaDanhGiaUngDung(string phoneNumber, string noiDung, int mucDo)
        //{
        //    // Retrieve the existing review based on the phoneNumber
        //    var existingReview = db.DanhGiaUngDungs.FirstOrDefault(d => d.SoDienThoai == phoneNumber);

        //    if (existingReview != null)
        //    {
        //        existingReview.NoiDung = noiDung;
        //        existingReview.MucDo = mucDo;
        //        db.Entry(existingReview).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    else
        //    {
        //        // Create a new review if it doesn't exist
        //        var khachHang = db.KhachHangs.FirstOrDefault(k => k.SoDienThoai == phoneNumber);
        //        if (khachHang != null)
        //        {
        //            existingReview = new DanhGiaUngDung
        //            {
        //                SoDienThoai = phoneNumber,
        //                NgayDanhGia = DateTime.Now,
        //                MucDo = mucDo,
        //                NoiDung = noiDung,
        //                KhachHang = khachHang
        //            };
        //            db.DanhGiaUngDungs.Add(existingReview);
        //            db.SaveChanges();
        //        }
        //    }

        //    // Create a response object with the updated review details
        //    var response = new
        //    {
        //        HinhAnh = existingReview?.KhachHang.HinhAnh,
        //        NgayDanhGia = existingReview?.NgayDanhGia.ToString("dd/MM/yyyy"),
        //        Ten = existingReview?.KhachHang.Ten,
        //        NoiDung = existingReview?.NoiDung,
        //        MucDo = existingReview?.MucDo
        //    };

        //    return Json(response);
        //}


        //public ActionResult XemDanhGiaUngDung(DanhGiaUngDung dgg)
        //{
        //    string a = Session["SoDienThoai"].ToString();
        //    var query = (from k in db.KhachHangs
        //                 join dg in db.DanhGiaUngDungs on k.SoDienThoai equals dg.SoDienThoai
        //                 where (dg.SoDienThoai == a)
        //                 select dg).ToList();
        //    //ViewBag.NoiDung = query;
        //    //ViewBag.MucDo = query;
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult SuaDanhGiaUngDung(DanhGiaUngDung dg)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(dg).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("DanhGiaUngDung");
        //    }
        //    return View();
        //}
    }
}