using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio.Types;
using WebsiteDatXeCongNghe.Models;
using WebsiteDatXeCongNghe.ViewModel.MultipleData;

namespace WebsiteDatXeCongNghe.Controllers
{
    public class ThanhToanController : Controller
    {
        private DichVuDatXeWebsite3Entities db = new DichVuDatXeWebsite3Entities();
        // GET: ThanhToan
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemThanhToan(string PassengerPhone, string DriverPhone, int MaDatXe, decimal GiaBanDau, decimal PhiCauDuong, decimal PhiMoRong, decimal PhiKhuyenMai, decimal ThanhTien, string HinhThucThanhToan, bool TrangThai, string SoThe)
        {
            ThanhToan tt = new ThanhToan();
            tt.SoDienThoai = PassengerPhone;
            tt.SoDienThoaiTX = DriverPhone;
            tt.MaDatXe = MaDatXe;
            tt.GiaBanDau = GiaBanDau;
            tt.PhiCauDuong = PhiCauDuong;
            tt.PhiMoRong = PhiMoRong;
            tt.PhiKhuyenMai = PhiKhuyenMai;
            tt.TongTien = ThanhTien;
            tt.HinhThucThanhToan = HinhThucThanhToan;
            tt.TrangThai = TrangThai;
            tt.NgayThanhToan = DateTime.Now;
            db.ThanhToans.Add(tt);
            db.SaveChanges();

            // Update the DiemUyTin in the TaiXe table based on the rating value
            var driver = db.TaiXes.SingleOrDefault(x => x.SoDienThoai == DriverPhone);

            if (driver != null)
            {
                decimal diemUyTinToAdd = 0.0M;


                diemUyTinToAdd = 0.5M;
                
                

                // Update DiemUyTin in TaiXe table
                driver.DiemUyTin = driver.DiemUyTin + diemUyTinToAdd;

                // Handle wallet update based on payment method
                if (HinhThucThanhToan == "Tiền mặt")
                {
                    // Subtract 30% of ThanhTien from the ViTien column
                    decimal amountToSubtract = ThanhTien * 0.3M;
                    driver.ViTien = driver.ViTien - amountToSubtract;
                }
                else if (HinhThucThanhToan == "Qua thẻ")
                {
                    // Add 70% of ThanhTien to the ViTien column
                    decimal amountToAdd = ThanhTien * 0.7M;
                    driver.ViTien = driver.ViTien + amountToAdd;
                    // Check if the TaiXe exists
                    var KH = db.TaiXes.FirstOrDefault(t => t.SoDienThoai == DriverPhone);
                    // Check if the TaiKhoanNganHang exists
                    var taiKhoan = db.TaiKhoanNganHangs.FirstOrDefault(t => t.SoThe == SoThe);

                    // Check if the amount is greater than the balance in TheVisa
                    var visaAccount = db.TheVisas.FirstOrDefault(v => v.SoThe == SoThe);

                    // Deduct the amount from the SoDu column in the TaiKhoanNganHang table
                    taiKhoan.SoDu -= ThanhTien;
                    var formattedBalanceCard = taiKhoan.SoDu.ToString("F3");
                    // Deduct the amount from the SoDu column in the TheVisa table
                    visaAccount.SoDu -= ThanhTien;
                }
                db.SaveChanges();
            }

            //var LichSuDat = db.ThongTinDatXes.SingleOrDefault(x => x.MaDatXe == MaDatXe);

            //if (LichSuDat != null)
            //{
            //    LichSuDat.TrangThai = true;
            //    db.SaveChanges();
            //}

            return View();
        }

        [HttpPost]
        public ActionResult ThemThanhToan2(string PassengerPhone, string DriverPhone, int MaDatXe, decimal GiaBanDau, decimal PhiCauDuong, decimal PhiMoRong, decimal PhiKhuyenMai, decimal ThanhTien, string HinhThucThanhToan, bool TrangThai, string SoThe)
        {
            ThanhToan tt = new ThanhToan();
            tt.SoDienThoai = PassengerPhone;
            tt.SoDienThoaiTX = DriverPhone;
            tt.MaDatXe = MaDatXe;
            tt.GiaBanDau = GiaBanDau;
            tt.PhiCauDuong = PhiCauDuong;
            tt.PhiMoRong = PhiMoRong;
            tt.PhiKhuyenMai = PhiKhuyenMai;
            tt.TongTien = ThanhTien;
            tt.HinhThucThanhToan = HinhThucThanhToan;
            tt.TrangThai = TrangThai;
            tt.NgayThanhToan = DateTime.Now;
            db.ThanhToans.Add(tt);
            db.SaveChanges();

            // Update the DiemUyTin in the TaiXe table based on the rating value
            var driver = db.TaiXes.SingleOrDefault(x => x.SoDienThoai == DriverPhone);

            if (driver != null)
            {
                decimal diemUyTinToAdd = 0.0M;


                diemUyTinToAdd = 1M;



                // Update DiemUyTin in TaiXe table
                driver.DiemUyTin = driver.DiemUyTin + diemUyTinToAdd;

                // Handle wallet update based on payment method
                if (HinhThucThanhToan == "Tiền mặt")
                {
                    // Subtract 30% of ThanhTien from the ViTien column
                    decimal amountToSubtract = ThanhTien * 0.3M;
                    driver.ViTien = driver.ViTien - amountToSubtract;
                }
                else if (HinhThucThanhToan == "Qua thẻ")
                {
                    // Add 70% of ThanhTien to the ViTien column
                    decimal amountToAdd = ThanhTien * 0.7M;
                    driver.ViTien = driver.ViTien + amountToAdd;
                    // Check if the TaiXe exists
                    var KH = db.TaiXes.FirstOrDefault(t => t.SoDienThoai == DriverPhone);
                    // Check if the TaiKhoanNganHang exists
                    var taiKhoan = db.TaiKhoanNganHangs.FirstOrDefault(t => t.SoThe == SoThe);

                    // Check if the amount is greater than the balance in TheVisa
                    var visaAccount = db.TheVisas.FirstOrDefault(v => v.SoThe == SoThe);

                    // Deduct the amount from the SoDu column in the TaiKhoanNganHang table
                    taiKhoan.SoDu -= ThanhTien;
                    var formattedBalanceCard = taiKhoan.SoDu.ToString("F3");
                    // Deduct the amount from the SoDu column in the TheVisa table
                    visaAccount.SoDu -= ThanhTien;
                }
                db.SaveChanges();
            }

            var LichSuDat = db.ThongTinDatXes.SingleOrDefault(x => x.MaDatXe == MaDatXe);

            if (LichSuDat != null)
            {
                LichSuDat.TrangThai = true;
                db.SaveChanges();
            }

            return View();
        }

        public ActionResult ThongTinThanhToan(ThanhToan tt)
        {            
            if (Session["SoDienThoai"] != null)
            {
                string a = Session["SoDienThoai"].ToString();
                var query = (from d in db.ThanhToans
                             join t in db.KhachHangs on d.SoDienThoai equals t.SoDienThoai
                             where (d.SoDienThoai == a)
                             select d)
                             .OrderByDescending(d => d.MaThanhToan)
                             .ToList();
                return View(query);
            }
            else
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
        }

        [HttpGet]
        public ActionResult ChiTietThanhToan(int id)
        {
            var viewModel = new MultipleData();
            viewModel.thanhToans = db.ThanhToans.Where(tt => tt.MaDatXe == id).ToList();
            viewModel.thongTinDatXes = db.ThongTinDatXes.Where(tt => tt.MaDatXe == id).ToList();
            return View(viewModel);
        }

        public ActionResult ThongTinThanhToanTX(ThanhToan tt)
        {           
            if (Session["SoDienThoai"] != null)
            {
                string a = Session["SoDienThoai"].ToString();
                var query = (from d in db.ThanhToans
                             join t in db.TaiXes on d.SoDienThoaiTX equals t.SoDienThoai
                             where (d.SoDienThoaiTX == a)
                             select d)
                             .OrderByDescending(d => d.MaThanhToan)
                             .ToList();
                return View(query);
            }
            else
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
        }

        [HttpGet]
        public ActionResult ChiTietThanhToanTX(int id)
        {
            var viewModel = new MultipleData();
            viewModel.thanhToans = db.ThanhToans.Where(tt => tt.MaDatXe == id).ToList();
            viewModel.thongTinDatXes = db.ThongTinDatXes.Where(tt => tt.MaDatXe == id).ToList();
            return View(viewModel);
        }
    }
}