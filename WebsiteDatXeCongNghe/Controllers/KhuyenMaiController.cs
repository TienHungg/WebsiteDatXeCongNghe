using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteDatXeCongNghe.Models;

namespace WebsiteDatXeCongNghe.Controllers
{
    public class KhuyenMaiController : Controller
    {
        private DichVuDatXeWebsite3Entities db = new DichVuDatXeWebsite3Entities();
        // GET: KhuyenMai
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetPromotionalCodes()
        {
            // Retrieve data from the database
            var promotionalCodes = db.KhuyenMais
                .Where(km => km.NgayKetThuc >= DateTime.Now)
                .Select(km => new
                {
                    MaCode = km.MaCode,
                    TenKhuyenMai = km.TenKhuyenMai,
                    PhanTram = km.PhanTram
                })
                .ToList();

            // Update TrangThai based on NgayKetThuc
            foreach (var promotionalCode in promotionalCodes)
            {
                var km = db.KhuyenMais.FirstOrDefault(k => k.MaCode == promotionalCode.MaCode);
                if (km != null)
                {
                    if (km.NgayKetThuc < DateTime.Now)
                    {
                        // Auto update TrangThai to false if NgayKetThuc < DateTime.Now
                        UpdateTrangThai(promotionalCode.MaCode, false);
                    }
                    else
                    {
                        // Auto update TrangThai to true if NgayKetThuc >= DateTime.Now
                        UpdateTrangThai(promotionalCode.MaCode, true);
                    }
                }
            }

            return Json(promotionalCodes, JsonRequestBehavior.AllowGet);
        }


        private void UpdateTrangThai(string maCode, bool trangThai)
        {
            var khuyenMai = db.KhuyenMais.FirstOrDefault(km => km.MaCode == maCode);
            if (khuyenMai != null)
            {
                khuyenMai.TrangThai = trangThai;
                db.SaveChanges();
            }
        }
    }
}