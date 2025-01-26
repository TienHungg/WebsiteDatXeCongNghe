using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebsiteDatXeCongNghe.Models;
using WebsiteDatXeCongNghe.ViewModel.MultipleData;
using Nexmo.Api;
using Twilio.Types;
using Client = Nexmo.Api.Client;

namespace WebsiteDatXeCongNghe.Controllers
{
    public class TaiXeController : Controller
    {
        private DichVuDatXeWebsite3Entities db = new DichVuDatXeWebsite3Entities();
        // GET: TaiXe
        public ActionResult ThongTinTaiXe()
        {            
            if (Session["SoDienThoai"] != null)
            {
                string a = Session["SoDienThoai"].ToString();
                var query = (from k in db.TaiXes
                             join t in db.TaiKhoans on k.SoDienThoai equals t.SoDienThoai
                             where (k.SoDienThoai == a)
                             select k).ToList();
                return View(query);
            }
            else
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
        }
        public ActionResult ThongTinCaNhanTaiXe()
        {            
            if (Session["SoDienThoai"] != null)
            {
                string a = Session["SoDienThoai"].ToString();
                var query = (from k in db.TaiXes
                             join t in db.TaiKhoans on k.SoDienThoai equals t.SoDienThoai
                             where (k.SoDienThoai == a)
                             select k).ToList();
                return View(query);
            }
            else
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
        }
        public ActionResult SuaDoiTX(int? id)
        {
            if (id == null)
            {
                // Handle the case when 'id' is null
                return RedirectToAction("ThongTinCaNhanTaiXe");
            }

            var TaiXe = db.TaiXes.FirstOrDefault(k => k.MaTX == id);
            if (TaiXe == null)
            {
                // Handle the case when 'TaiXe' is null
                return RedirectToAction("ThongTinCaNhanTaiXe");
            }

            var TaiXeDTO = new TaiXeDTO
            {
                MaTX = TaiXe.MaTX,
                SoDienThoai = TaiXe.SoDienThoai,
                Ten = TaiXe.Ten,
                NgayThangNamSinh = TaiXe.NgayThangNamSinh,
                Email = TaiXe.Email,
                BienSo = TaiXe.BienSo,
                CCCD = TaiXe.CCCD,
                HinhAnh = TaiXe.HinhAnh
            };

            return Json(TaiXeDTO, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult SuaDoiTX(TaiXe tx, HttpPostedFileBase HinhAnhUpload)
        {
            if (HinhAnhUpload != null)
            {
                string path = uploadimage(HinhAnhUpload);
                tx.HinhAnh = path;
            }

            if (ModelState.IsValid)
            {
                db.Entry(tx).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ThongTinCaNhanTaiXe");
            }

            return View(tx);
        }

        public string uploadimage(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/image/TaiXe"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "" + random + Path.GetFileName(file.FileName);
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg, jpeg, or png formats are acceptable....');</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Please select a file');</script>");
                path = "-1";
            }
            return path;
        }
        //public ActionResult ThongTinNhanChuyen()
        //{
        //    string a = Session["SoDienThoai"].ToString();
        //    var query = (from k in db.TaiXeNhanChuyenXes
        //                 join t in db.TaiXes on k.SoDienThoai equals t.SoDienThoai
        //                 where (k.SoDienThoai == a)
        //                 select k).ToList();
        //    return View(query);
        //}

        public ActionResult ThongTinNhanChuyen()
        {
            if (Session["SoDienThoai"] != null)
            {
                string phoneNumber = Session["SoDienThoai"].ToString();

                var mymodel = new MultipleData();

                // Retrieve top 3 rides for the specific driver's phone number
                var topRide = db.TaiXeNhanChuyenXes
                    .Where(t => t.SoDienThoai == phoneNumber)
                    .OrderByDescending(t => t.MaNhanChuyen)
                    .ToList();

                // Verify top3Ride contains data
                if (topRide.Count > 0)
                {
                    // Extract MaDatXe values from top3Ride
                    var maDatXes = topRide.Select(t => t.MaDatXe).ToList();

                    // Load ThongTinDatXes for corresponding MaDatXe in the rides
                    mymodel.thongTinDatXes = db.ThongTinDatXes.Where(tt => maDatXes.Contains(tt.MaDatXe)).ToList();

                    // Load rides
                    mymodel.taiXeNhanChuyenXes = topRide;
                }

                return View(mymodel);
            }
            else
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
        }

        [HttpGet]
        public ActionResult ChiTietThongTinNhanChuyen(int id)
        {
            var taiXeNhanChuyenXes = db.TaiXeNhanChuyenXes
                .Where(tt => tt.MaDatXe == id)
                .Select(tt => new
                {
                    tt.MaNhanChuyen,
                    tt.SoDienThoai,
                    tt.NgayNhan,
                    // Add other necessary properties
                })
                .ToList();

            var thongTinDatXes = db.ThongTinDatXes
                .Where(tt => tt.MaDatXe == id)
                .Select(tt => new
                {
                    tt.MaDatXe,
                    tt.SoDienThoai,
                    tt.DiemDon,
                    tt.DiemDen,
                    tt.SoKm,
                    tt.ThoiGian,
                    tt.ThanhTien,
                    tt.LoaiXe,
                    tt.HinhThucThanhToan,
                    tt.NgayGioDat,
                    tt.TrangThai,
                    tt.PassengerName,
                    // Add other necessary properties
                })
                .ToList();

            var result = new
            {
                success = true,
                detailRide = new
                {
                    taiXeNhanChuyenXes,
                    thongTinDatXes,
                }
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTrangThaiHoatDong(string driverPhone)
        {
            try
            {
                // Assuming db is your DbContext
                var trangThaiHoatDong = db.TrangThaiHoatDongs.FirstOrDefault(t => t.SoDienThoai == driverPhone);

                if (trangThaiHoatDong != null)
                {
                    // Return the TrangThai value
                    return Json(trangThaiHoatDong.TrangThai, JsonRequestBehavior.AllowGet);
                }

                return Json(null, JsonRequestBehavior.AllowGet); // Handle the case where the record is not found
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult UpdateTrangThaiHoatDong(string driverPhone, bool isChecked)
        {
            try
            {
                // Assuming db is your DbContext
                var trangThaiHoatDong = db.TrangThaiHoatDongs.FirstOrDefault(t => t.SoDienThoai == driverPhone);

                if (trangThaiHoatDong != null)
                {
                    // Update the TrangThai column based on the isChecked parameter
                    trangThaiHoatDong.TrangThai = isChecked;

                    // Save changes to the database
                    db.SaveChanges();

                    return Json(new { success = true, message = "TrangThaiHoatDong updated successfully" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, message = "TrangThaiHoatDong not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult TotalRide(string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                // Get the total ride count
                var totalRide = db.TaiXeNhanChuyenXes.Count(t => t.SoDienThoai == phoneNumber);

                return Json(new { success = true, totalRide = totalRide }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false });
        }

        public ActionResult TotalRideThisYear(string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                // Get the total ride count for this year
                var totalRideThisYear = db.TaiXeNhanChuyenXes.Count(t => t.SoDienThoai == phoneNumber && t.NgayNhan.Year == DateTime.Now.Year);

                return Json(new { success = true, totalRideThisYear = totalRideThisYear }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false });
        }

        public ActionResult TotalRideToday(string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                // Get the total ride count for today
                var todayStart = DateTime.Today;
                var todayEnd = todayStart.AddDays(1);
                var totalRideToday = db.TaiXeNhanChuyenXes.Count(t => t.SoDienThoai == phoneNumber && t.NgayNhan >= todayStart && t.NgayNhan < todayEnd);

                return Json(new { success = true, totalRideToday = totalRideToday }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false });
        }


        [HttpPost]
        public ActionResult XoaThongTinNhanChuyen(int id)
        {
            TaiXeNhanChuyenXe txnc = db.TaiXeNhanChuyenXes.Find(id);
            db.TaiXeNhanChuyenXes.Remove(txnc);
            db.SaveChanges();
            return RedirectToAction("ThongTinNhanChuyen");
        }

        //public ActionResult SuaDoiViTri(int id)
        //{

        //    KhachHang Khach = db.KhachHangs.Find(id);
        //    //var Khach = db.KhachHangs.Find(id);            
        //    return View(Khach);
        //}
        [HttpPost]
        public ActionResult SuaDoiViTri(string location, string phoneNumber)
        {
            TaiXe tx = db.TaiXes.SingleOrDefault(x => x.SoDienThoai == phoneNumber);
            if (tx != null)
            {
                tx.ViTri = location;
                db.Entry(tx).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        public ActionResult TaiXeDonKhach()
        {
            string a = Session["SoDienThoai"].ToString();
            var query = (from k in db.TaiXes
                         join t in db.TaiKhoans on k.SoDienThoai equals t.SoDienThoai
                         where (k.SoDienThoai == a)
                         select k).ToList();
            return View(query);
        }
        [HttpPost]
        public ActionResult SendDriverData(string PhoneNumber, string DriverLocations, string DriverName, string DriverLicense, string txtLocationPickup, string txtLocationDrop, string result, string totalFare, string PassengerPhone, string distance, string duration, string type, string dated, string DriverRate, string DriverImage, string PassengerImages, string PassengerName, string MaDatXe, string MaNhanChuyen, string SurchareFees, string discountFare, string discountFareTotal)
        {
            Session["PhoneNumber"] = PhoneNumber;
            Session["DriverLocations"] = DriverLocations;
            Session["DriverName"] = DriverName;
            Session["DriverLicense"] = DriverLicense;
            Session["txtLocationPickup"] = txtLocationPickup;
            Session["txtLocationDrop"] = txtLocationDrop;
            Session["result"] = result;
            Session["totalFare"] = totalFare;
            Session["PassengerPhone"] = PassengerPhone;
            Session["distance"] = distance;
            Session["duration"]=duration;
            Session["type"] = type;
            Session["dated"]=dated;
            Session["DriverRate"] = DriverRate;
            Session["DriverImage"] = DriverImage;
            Session["PassengerImages"]= PassengerImages;
            Session["PassengerName"] = PassengerName;
            Session["MaDatXe"] = MaDatXe;
            Session["MaNhanChuyen"] = MaNhanChuyen;
            Session["SurchareFees"] = SurchareFees;
            Session["discountFare"] = discountFare;
            Session["discountFareTotal"] = discountFareTotal;
            return Json(new { success = true }); // Return a JSON response with the driver's image and rating
        }
        [HttpPost]
        public ActionResult SendOTP()
        {
            return Json(new { success = true });
        }
        public ActionResult DonKhach()
        {
            string a = Session["SoDienThoai"].ToString();
            var query = (from k in db.TaiXes
                         join t in db.TaiKhoans on k.SoDienThoai equals t.SoDienThoai
                         where (k.SoDienThoai == a)
                         select k).ToList();
            return View(query);
        }
        public ActionResult TraKhach()
        {
            string a = Session["SoDienThoai"].ToString();
            var query = (from k in db.TaiXes
                         join t in db.TaiKhoans on k.SoDienThoai equals t.SoDienThoai
                         where (k.SoDienThoai == a)
                         select k).ToList();
            return View(query);
        }
        [HttpPost]
        public ActionResult TaiXeHuyChuyen(string driverPhoneNumber, int MaDatXe, bool TrangThai)
        {
            try
            {
                // Create a new review object
                TaiXeNhanChuyenXe txnc = new TaiXeNhanChuyenXe();
                txnc.SoDienThoai = driverPhoneNumber;
                txnc.MaDatXe = MaDatXe;
                txnc.NgayNhan = DateTime.Now;
                txnc.TrangThai = TrangThai;

                // Add the review to the database
                db.TaiXeNhanChuyenXes.Add(txnc);
                db.SaveChanges();


                // Get the driver from the database
                var driver = db.TaiXes.SingleOrDefault(x => x.SoDienThoai == driverPhoneNumber);

                if (driver != null)
                {
                    // Subtract 0.2 from DiemUyTin
                    driver.DiemUyTin = Math.Max(0, driver.DiemUyTin - 0.2M);

                    // Save changes to the database
                    db.SaveChanges();

                    // Return the updated driver data as JSON
                    return Json(new
                    {
                        Success = true,
                        Message = "Drive canceled successfully.",
                        Driver = new
                        {
                            DiemUyTin = driver.DiemUyTin,
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Handle the case where the driver is not found
                    return Json(new { Success = false, Message = "Driver not found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, etc.
                return Json(new { Success = false, Message = "An error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult TaiXeHuyChuyen2(string driverPhoneNumber)
        {
            try
            {
                // Get the driver from the database
                var driver = db.TaiXes.SingleOrDefault(x => x.SoDienThoai == driverPhoneNumber);

                if (driver != null)
                {
                    // Subtract 0.2 from DiemUyTin
                    driver.DiemUyTin = Math.Max(0, driver.DiemUyTin - 0.2M);

                    // Save changes to the database
                    db.SaveChanges();

                    // Return the updated driver data as JSON
                    return Json(new
                    {
                        Success = true,
                        Message = "Drive canceled successfully.",
                        Driver = new
                        {
                            DiemUyTin = driver.DiemUyTin,
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Handle the case where the driver is not found
                    return Json(new { Success = false, Message = "Driver not found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, etc.
                return Json(new { Success = false, Message = "An error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult TaiXeNhanChuyen(string driverPhoneNumber, int MaDatXe, bool TrangThai)
        {
            TaiXeNhanChuyenXe txnc = new TaiXeNhanChuyenXe();
            txnc.SoDienThoai = driverPhoneNumber;
            txnc.MaDatXe = MaDatXe;
            txnc.NgayNhan = DateTime.Now;
            txnc.TrangThai = TrangThai;

            // Add the review to the database
            db.TaiXeNhanChuyenXes.Add(txnc);
            db.SaveChanges();

            // Return the MaNhanChuyen value as a JSON response
            return Json(new { MaNhanChuyen = txnc.MaNhanChuyen });
        }


        [HttpPost]
        public ActionResult TrangThaiNhanChuyen(int MaNhanChuyen, bool TrangThai)
        {
            try
            {               
                // Get the driver from the database
                var driver = db.TaiXeNhanChuyenXes.SingleOrDefault(x => x.MaNhanChuyen == MaNhanChuyen);

                if (driver != null)
                {
                    // Subtract 0.2 from DiemUyTin
                    driver.TrangThai = TrangThai;

                    // Save changes to the database
                    db.SaveChanges();
                }
                else
                {
                    // Handle the case where the driver is not found
                    return Json(new { Success = false, Message = "Driver not found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, etc.
                return Json(new { Success = false, Message = "An error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return View();
        }



        public ActionResult GetDriversWithRating()
        {
            // Define the minimum driver rating threshold
            decimal minDriverRating = 3.0M; // Minimum rating threshold

            var drivers = db.TaiXes.Where(driver => driver.MucDoDanhGiaTB >= minDriverRating).ToList();

            return View(drivers);
        }

        [HttpGet]
        public ActionResult FilterPhoneNumberofTaiXe()
        {
            // Condition 1: Retrieve drivers with TrangThai set to true from TrangThaiHoatDong table
            var activeDrivers = db.TrangThaiHoatDongs
                .Where(tt => (bool)tt.TrangThai)
                .Select(tt => tt.SoDienThoai) // Select phone numbers of active drivers
                .ToList();

            // Condition 2: Retrieve drivers from TaiXe with MucDoDanhGiaTB higher than or equal to 3
            var highRatedDrivers = db.TaiXes
                .Where(tx => tx.MucDoDanhGiaTB >= 3)
                .Select(tx => tx.SoDienThoai) // Select phone numbers of high-rated drivers
                .ToList();

            // Select the phone numbers of drivers with the highest rating
            var eligibleDrivers = db.TaiXes
                .Where(tx => activeDrivers.Contains(tx.SoDienThoai) && highRatedDrivers.Contains(tx.SoDienThoai))
                .Select(tx => new
                {
                    tx.SoDienThoai,
                    tx.ViTri,
                    tx.MucDoDanhGiaTB
                })
                .ToList();


            return Json(eligibleDrivers, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDriverLocations()
        {
            // Condition 1: Retrieve drivers with TrangThai set to true from TrangThaiHoatDong table
            var activeDrivers = db.TrangThaiHoatDongs
                .Where(tt => (bool)tt.TrangThai)
                .Select(tt => tt.SoDienThoai)
                .ToList();

            // Get the list of eligible drivers based on your criteria (active and high-rated drivers)
            var driverLocations = db.TaiXes
                .Where(tx => activeDrivers.Contains(tx.SoDienThoai))
                .Select(tx => new
                {
                    tx.SoDienThoai,
                    tx.ViTri
                })
                .ToList();

            // Return the list of drivers along with their locations as JSON
            return Json(driverLocations, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QuanLy()
        {
            var mymodel = new MultipleData();
            mymodel.khachHangs = db.KhachHangs.ToList();
            mymodel.thongTinDatXes = db.ThongTinDatXes.ToList();
            mymodel.taiKhoans = db.TaiKhoans.ToList();
            mymodel.taiXes = db.TaiXes.ToList();
            mymodel.danhGias = db.DanhGiaUngDungs.ToList();
            mymodel.khuyenMais = db.KhuyenMais.ToList();
            mymodel.thanhToans = db.ThanhToans.ToList();
            mymodel.danhGiaTaiXes = db.DanhGiaTaiXes.ToList();
            mymodel.taiXeNhanChuyenXes = db.TaiXeNhanChuyenXes.ToList();
            mymodel.taiKhoanNganHangs = db.TaiKhoanNganHangs.ToList();
            mymodel.quyenTaiKhoans = db.QuyenTaiKhoans.ToList();
            mymodel.phanQuyens = db.PhanQuyens.ToList();
            return View(mymodel);
        }

        [HttpGet]
        public ActionResult ViTienTaiXe()
        {
            if (Session["SoDienThoai"] != null)
            {
                string phoneNumber = Session["SoDienThoai"].ToString();

                var model = new MultipleData();

                // Populate the TaiXes property
                model.taiXes = (from k in db.TaiXes
                                join t in db.TaiKhoans on k.SoDienThoai equals t.SoDienThoai
                                where (k.SoDienThoai == phoneNumber)
                                select k).ToList();

                // Populate the TaiXes property
                model.taiKhoanNganHangTaiXes = (from k in db.TaiKhoanNganHangTaiXes
                                                join t in db.TaiXes on k.SoDienThoai equals t.SoDienThoai
                                where (k.SoDienThoai == phoneNumber)
                                select k).ToList();
                // Add additional properties if needed

                return View(model);
            }
            else
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
        }

        [HttpGet]
        public ActionResult GetViTien()
        {
            // Retrieve ViTien value based on the logged-in user's phone number
            if (Session["SoDienThoai"] != null)
            {
                string phoneNumber = Session["SoDienThoai"].ToString();

                var viTien = db.TaiXes
                    .Where(t => t.SoDienThoai == phoneNumber)
                    .Select(t => t.ViTien)
                    .FirstOrDefault();

                return Content(viTien.ToString());
            }
            else
            {
                return Content("0"); // Assuming a default value if user is not logged in
            }
        }

        [HttpGet]
        public ActionResult GetIDViTien()
        {
            // Retrieve ViTien value based on the logged-in user's phone number
            if (Session["SoDienThoai"] != null)
            {
                string phoneNumber = Session["SoDienThoai"].ToString();

                var idViTien = db.TaiXes
                    .Where(t => t.SoDienThoai == phoneNumber)
                    .Select(t => t.MaTX)
                    .FirstOrDefault();

                return Content(idViTien.ToString());
            }
            else
            {
                return Content("0"); // Assuming a default value if user is not logged in
            }
        }

        [HttpPost]
        public JsonResult GetTotalWithdrawalAmount(string phoneNumber)
        {
            try
            {
                // Ensure phoneNumber is not null or empty
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    return Json(new { success = false, message = "Invalid phone number." });
                }

                // Get the total withdrawal amount for the specified phone number
                decimal totalWithdrawalAmount = db.LichSuGiaoDiches
                    .Where(l => l.SoDienThoai == phoneNumber && l.LoaiGiaoDich == "Withdrawal")
                    .Sum(l => (decimal?)l.SoTien) ?? 0;

                return Json(new { success = true, totalWithdrawalAmount });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Json(new { success = false, message = "Error while fetching total withdrawal amount." });
            }
        }

        [HttpPost]
        public JsonResult GetTotalDepositAmount(string phoneNumber)
        {
            try
            {
                // Ensure phoneNumber is not null or empty
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    return Json(new { success = false, message = "Invalid phone number." });
                }

                // Get the total Deposit amount for the specified phone number
                decimal totalDepositAmount = db.LichSuGiaoDiches
                    .Where(l => l.SoDienThoai == phoneNumber && l.LoaiGiaoDich == "Deposit")
                    .Sum(l => (decimal?)l.SoTien) ?? 0;

                return Json(new { success = true, totalDepositAmount });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Json(new { success = false, message = "Error while fetching total Deposit amount." });
            }
        }

        [HttpGet]
        public JsonResult GetNumberOfWithdrawals(string phoneNumber)
        {
            try
            {
                // Ensure phoneNumber is not null or empty
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    return Json(new { success = false, numberOfWithdrawals = 0 });
                }

                // Query the number of withdrawals for the specified phone number
                var numberOfWithdrawals = db.LichSuGiaoDiches
                    .Where(x => x.SoDienThoai == phoneNumber && x.LoaiGiaoDich == "Withdrawal")
                    .Count();

                return Json(new { success = true, numberOfWithdrawals }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                // Log or handle the exception as needed
                return Json(new { success = false, numberOfWithdrawals = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetNumberOfDeposit(string phoneNumber)
        {
            try
            {
                // Ensure phoneNumber is not null or empty
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    return Json(new { success = false, numberOfDeposit = 0 });
                }

                // Query the number of Deposit for the specified phone number
                var numberOfDeposit = db.LichSuGiaoDiches
                    .Where(x => x.SoDienThoai == phoneNumber && x.LoaiGiaoDich == "Deposit")
                    .Count();

                return Json(new { success = true, numberOfDeposit }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                // Log or handle the exception as needed
                return Json(new { success = false, numberOfDeposit = 0 }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult GetSoDuAndSoThe()
        {
            if (Session["SoDienThoai"] != null)
            {
                string phoneNumber = Session["SoDienThoai"].ToString();

                var taiKhoanNganHang = db.TaiKhoanNganHangTaiXes
                    .Where(t => t.SoDienThoai == phoneNumber)
                    .FirstOrDefault();

                if (taiKhoanNganHang != null)
                {
                    return Json(new { soDu = taiKhoanNganHang.SoDu, soThe = taiKhoanNganHang.SoThe }, JsonRequestBehavior.AllowGet);
                }
            }

            // Return default values if the user is not logged in or no data is found
            return Json(new { soDu = "0", soThe = "" }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetSoDu(int? maTaiKhoan)
        {
            if (maTaiKhoan.HasValue)
            {
                // Retrieve SoDu value based on the provided MaTaiKhoanNHTaiXe
                var soDu = db.TaiKhoanNganHangTaiXes
                    .Where(t => t.MaTaiKhoanNHTaiXe == maTaiKhoan.Value)
                    .Select(t => t.SoDu)
                    .FirstOrDefault();

                return Content(soDu.ToString());
            }
            else
            {
                // Retrieve overall SoDu value based on the logged-in user's phone number
                if (Session["SoDienThoai"] != null)
                {
                    string phoneNumber = Session["SoDienThoai"].ToString();

                    var soDu = db.TaiKhoanNganHangTaiXes
                        .Where(t => t.SoDienThoai == phoneNumber)
                        .Select(t => t.SoDu)
                        .FirstOrDefault();

                    return Content(soDu.ToString());
                }
                else
                {
                    return Content("0"); // Assuming a default value if the user is not logged in
                }
            }
        }

        [HttpGet]
        public ActionResult GetSoThe(int? maTaiKhoan)
        {
            if (maTaiKhoan.HasValue)
            {
                // Retrieve SoDu value based on the provided MaTaiKhoanNHTaiXe
                var soThe = db.TaiKhoanNganHangTaiXes
                    .Where(t => t.MaTaiKhoanNHTaiXe == maTaiKhoan.Value)
                    .Select(t => t.SoThe)
                    .FirstOrDefault();

                // Check if soThe is null, and return "0" if it is
                return Content(soThe ?? "0");
            }
            else
            {
                // Retrieve overall SoDu value based on the logged-in user's phone number
                if (Session["SoDienThoai"] != null)
                {
                    string phoneNumber = Session["SoDienThoai"].ToString();

                    var soThe = db.TaiKhoanNganHangTaiXes
                        .Where(t => t.SoDienThoai == phoneNumber)
                        .Select(t => t.SoThe)
                        .FirstOrDefault();

                    // Check if soThe is null, and return "0" if it is
                    return Content(soThe ?? "0");
                }
                else
                {
                    return Content("0"); // Assuming a default value if the user is not logged in
                }
            }
        }


        //[HttpGet]
        //public ActionResult GetSoThe(int? maTaiKhoan)
        //{
        //    if (maTaiKhoan.HasValue)
        //    {
        //        // Retrieve SoThe value based on the provided MaTaiKhoanNHTaiXe
        //        var soThe = db.TaiKhoanNganHangTaiXes
        //            .Where(t => t.MaTaiKhoanNHTaiXe == maTaiKhoan.Value)
        //            .Select(t => t.SoThe)
        //            .FirstOrDefault();

        //        return Content(soThe);
        //    }
        //    else
        //    {
        //        return Content(""); // Return empty string if no MaTaiKhoan is provided
        //    }
        //}


        [HttpGet]
        public ActionResult TaiKhoanNganHangTX()
        {
            if (Session["SoDienThoai"] != null)
            {
                string phoneNumber = Session["SoDienThoai"].ToString();

                var model = new MultipleData();

                // Populate the TaiXes property
                model.taiXes = (from k in db.TaiXes
                                join t in db.TaiKhoans on k.SoDienThoai equals t.SoDienThoai
                                where (k.SoDienThoai == phoneNumber)
                                select k).ToList();

                // Add additional properties if needed

                return View(model);
            }
            else
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
        }

        


        [HttpPost]
        public ActionResult NapTien(decimal amount, string PhoneNumber, string SoThe)
        {
            try
            {
                var taiXe = db.TaiXes.FirstOrDefault(t => t.SoDienThoai == PhoneNumber);
                var taiKhoan = db.TaiKhoanNganHangTaiXes.FirstOrDefault(t => t.SoThe == SoThe);
                var visaAccount = db.TheVisas.FirstOrDefault(v => v.SoThe == SoThe);
                // Validate the amount and input parameters if needed
                if (amount < 10.000m)
                {
                    return Json(new { success = false, message = "Số tiền phải lớn hơn hoặc bằng 10.000 VNĐ." });
                }
                // Check if the amount is greater than the balance in TaiKhoanNganHangTaiXe
                if (amount > taiKhoan.SoDu)
                {
                    return Json(new { success = false, message = "Không đủ tiền trong số dư thẻ." });
                }

                // Check if the amount is greater than the balance in TheVisa

                if (visaAccount == null || amount > visaAccount.SoDu)
                {
                    return Json(new { success = false, message = "Không đủ tiền trong tài khoản Visa." });
                }

                // Check if the TaiXe exists

                if (taiXe == null)
                {
                    return Json(new { success = false, message = "Driver not found." });
                }

                // Check if the TaiKhoanNganHangTaiXe exists
                
                if (taiKhoan == null)
                {
                    return Json(new { success = false, message = "Bank account not found." });
                }

                string confirmationCode = GenerateConfirmationCode();

                // Check if phoneNumber contains "@"
                if (PhoneNumber.Contains("@"))
                {
                    bool sentSuccessfully = SendConfirmationCodeByEmail(PhoneNumber, confirmationCode);

                    if (!sentSuccessfully)
                    {
                        return Json(new { success = false, message = "Failed to send confirmation code via email" });
                    }
                }
                else
                {
                    bool sentSuccessfully = SendConfirmationCodeBySMS(PhoneNumber, confirmationCode);

                    if (!sentSuccessfully)
                    {
                        return Json(new { success = false, message = "Failed to send confirmation code via SMS" });
                    }
                }

                // Store the confirmation code in Session
                Session["ConfirmationCode"] = confirmationCode;

                return Json(new { success = true, message = "Gửi mã OTP!", confirmationCode });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }


        [HttpPost]
        public ActionResult NapTien2(decimal amount, string PhoneNumber, string SoThe)
        {
            try
            {
                // Check if the TaiXe exists
                var taiXe = db.TaiXes.FirstOrDefault(t => t.SoDienThoai == PhoneNumber);
                
                // Check if the TaiKhoanNganHangTaiXe exists
                var taiKhoan = db.TaiKhoanNganHangTaiXes.FirstOrDefault(t => t.SoThe == SoThe);
                
                // Check if the amount is greater than the balance in TheVisa
                var visaAccount = db.TheVisas.FirstOrDefault(v => v.SoThe == SoThe);
                
                // Update the wallet balance in the TaiXe table
                taiXe.ViTien += amount;

                // Format the wallet balance with three decimal places
                var formattedWalletBalance = taiXe.ViTien.ToString("F3");


                // Deduct the amount from the SoDu column in the TaiKhoanNganHangTaiXe table
                taiKhoan.SoDu -= amount;
                var formattedBalanceCard = taiKhoan.SoDu.ToString("F3");
                // Deduct the amount from the SoDu column in the TheVisa table
                visaAccount.SoDu -= amount;

                // Log the deposit transaction into LichSuGiaoDich table
                var transaction = new LichSuGiaoDich
                {
                    SoDienThoai = PhoneNumber,
                    NgayGiaoDich = DateTime.Now,
                    LoaiGiaoDich = "Deposit", // Assuming "Deposit" is the transaction type for deposits
                    SoTien = amount
                };

                db.LichSuGiaoDiches.Add(transaction);

                var formattedBalanceHistory = transaction.SoTien.ToString("F3");

                // Save changes to the database
                db.SaveChanges();

                // Return the updated data in the response
                return Json(new
                {
                    success = true,
                    message = "Nạp thành công vào ví với số tiền: ",
                    walletDriver = formattedWalletBalance,
                    balanceCard = formattedBalanceCard,
                    balanceHistory = formattedBalanceHistory
                });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public ActionResult RutTien(decimal amount, string PhoneNumber, string SoThe)
        {
            try
            {
                var taiXe = db.TaiXes.FirstOrDefault(t => t.SoDienThoai == PhoneNumber);
                var taiKhoan = db.TaiKhoanNganHangTaiXes.FirstOrDefault(t => t.SoThe == SoThe);
                var visaAccount = db.TheVisas.FirstOrDefault(v => v.SoThe == SoThe);
                // Validate the amount and input parameters if needed
                if (amount < 10.000m)
                {
                    return Json(new { success = false, message = "Số tiền phải lớn hơn hoặc bằng 10.000 VNĐ." });
                }
                // Check if the amount is greater than the wallet
                if (amount > taiXe.ViTien)
                {
                    return Json(new { success = false, message = "Không đủ tiền trong ví." });
                }

                //// Check if the amount is greater than the balance in TheVisa

                //if (visaAccount == null || amount > visaAccount.SoDu)
                //{
                //    return Json(new { success = false, message = "Không đủ tiền trong tài khoản Visa." });
                //}

                // Check if the TaiXe exists

                if (taiXe == null)
                {
                    return Json(new { success = false, message = "Driver not found." });
                }

                // Check if the TaiKhoanNganHangTaiXe exists

                if (taiKhoan == null)
                {
                    return Json(new { success = false, message = "Bank account not found." });
                }

                string confirmationCode = GenerateConfirmationCode();

                // Check if phoneNumber contains "@"
                if (PhoneNumber.Contains("@"))
                {
                    bool sentSuccessfully = SendConfirmationCodeByEmail(PhoneNumber, confirmationCode);

                    if (!sentSuccessfully)
                    {
                        return Json(new { success = false, message = "Failed to send confirmation code via email" });
                    }
                }
                else
                {
                    bool sentSuccessfully = SendConfirmationCodeBySMS(PhoneNumber, confirmationCode);

                    if (!sentSuccessfully)
                    {
                        return Json(new { success = false, message = "Failed to send confirmation code via SMS" });
                    }
                }

                // Store the confirmation code in Session
                Session["ConfirmationCode"] = confirmationCode;

                return Json(new { success = true, message = "Gửi mã OTP!", confirmationCode });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }


        [HttpPost]
        public ActionResult RutTien2(decimal amount, string PhoneNumber, string SoThe)
        {
            try
            {
                // Check if the TaiXe exists
                var taiXe = db.TaiXes.FirstOrDefault(t => t.SoDienThoai == PhoneNumber);

                // Check if the TaiKhoanNganHangTaiXe exists
                var taiKhoan = db.TaiKhoanNganHangTaiXes.FirstOrDefault(t => t.SoThe == SoThe);

                // Check if the amount is greater than the balance in TheVisa
                var visaAccount = db.TheVisas.FirstOrDefault(v => v.SoThe == SoThe);

                // Update the wallet balance in the TaiXe table
                taiXe.ViTien -= amount;

                // Format the wallet balance with three decimal places
                var formattedWalletBalance = taiXe.ViTien.ToString("F3");


                // Deduct the amount from the SoDu column in the TaiKhoanNganHangTaiXe table
                taiKhoan.SoDu += amount;
                var formattedBalanceCard = taiKhoan.SoDu.ToString("F3");
                // Deduct the amount from the SoDu column in the TheVisa table
                visaAccount.SoDu += amount;

                // Log the withdrawal transaction into LichSuGiaoDich table
                var transaction = new LichSuGiaoDich
                {
                    SoDienThoai = PhoneNumber,
                    NgayGiaoDich = DateTime.Now,
                    LoaiGiaoDich = "Withdrawal", // Assuming "Withdrawal" is the transaction type for withdrawal
                    SoTien = amount
                };

                db.LichSuGiaoDiches.Add(transaction);

                var formattedBalanceHistory = transaction.SoTien.ToString("F3");

                // Save changes to the database
                db.SaveChanges();

                // Return the updated data in the response
                return Json(new
                {
                    success = true,
                    message = "Rút thành công từ ví với số tiền: ",
                    walletDriver = formattedWalletBalance,
                    balanceCard = formattedBalanceCard,
                    balanceHistory = formattedBalanceHistory
                });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }




        //[HttpGet]
        //public ActionResult ViTienTaiXe()
        //{
        //    if (Session["SoDienThoai"] != null)
        //    {
        //        string a = Session["SoDienThoai"].ToString();
        //        var query = (from k in db.TaiXes
        //                     join t in db.TaiKhoans on k.SoDienThoai equals t.SoDienThoai
        //                     where (k.SoDienThoai == a)
        //                     select k).ToList();
        //        return View(query);
        //    }
        //    else
        //    {
        //        return RedirectToAction("DangNhap", "TaiKhoan");
        //    }
        //}

        [HttpPost]
        public JsonResult ValidateAndInsertCard(string phoneNumber, string cardNumber, string cardHolder, int month, int year, int cvv)
        {
            try
            {
                // Validate if the Visa card data exists in TheVisa table
                var visaCardData = db.TheVisas.SingleOrDefault(v => v.SoThe == cardNumber);

                if (visaCardData == null)
                {
                    // Visa card does not exist
                    return Json(new { success = false, message = "Số thẻ không tồn tại!" });
                }

                // Validate other information
                if (visaCardData.ThoiHanThang != month)
                {
                    // Visa card information does not match
                    return Json(new { success = false, message = "Thời hạn tháng không hợp lệ!" });
                }

                // Validate other information
                if (visaCardData.ThoiHanNam != year)
                {
                    // Visa card information does not match
                    return Json(new { success = false, message = "Thời hạn năm không hợp lệ!" });
                }

                // Validate other information
                if (visaCardData.CVV != cvv)
                {
                    // Visa card information does not match
                    return Json(new { success = false, message = "CVV không hợp lệ!" });
                }

                // Insert the record into TaiKhanNganHangTaiXe table
                var taiKhoanNganHang = new TaiKhoanNganHangTaiXe
                {
                    SoDienThoai = phoneNumber,
                    SoThe = cardNumber,
                    ChuThe = visaCardData.ChuThe, // Automatically populate ChuThe from TheVisa
                    ThoiHanThang = month,
                    ThoiHanNam = year,
                    CVV = cvv,
                    SoDu = (decimal)visaCardData.SoDu // Populate SoDu from TheVisa
                };

                db.TaiKhoanNganHangTaiXes.Add(taiKhoanNganHang);
                db.SaveChanges();

                return Json(new { success = true, message = $"Thẻ đăng ký thành công! Số dư hiện tại: {visaCardData.SoDu}" + " VND" });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return Json(new { success = false, message = "Đã xảy ra lỗi trong khi xử lý yêu cầu của bạn." });
            }
        }

        [HttpGet]
        public JsonResult GetChuTheByCardNumber(string cardNumber)
        {
            try
            {
                var chuThe = db.TheVisas.Where(v => v.SoThe == cardNumber).Select(v => v.ChuThe).FirstOrDefault();
                return Json(chuThe, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetMonthByCardNumber(string cardNumberMonth)
        {
            try
            {
                var thoiHanThang = db.TheVisas.Where(v => v.SoThe == cardNumberMonth).Select(v => v.ThoiHanThang).FirstOrDefault();
                return Json(thoiHanThang, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetYearByCardNumber(string cardNumberYear)
        {
            try
            {
                var thoiHanNam = db.TheVisas.Where(v => v.SoThe == cardNumberYear).Select(v => v.ThoiHanNam).FirstOrDefault();
                return Json(thoiHanNam, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ValidateVisaCard(string phoneNumber, string cardNumber, string cardHolder, int month, int year, int cvv)
        {
            try
            {
                // Validate if the Visa card data exists in TheVisa table
                var visaCardData = db.TheVisas.SingleOrDefault(v => v.SoThe == cardNumber);
                var visaCardData2 = db.TaiKhoanNganHangTaiXes.SingleOrDefault(v => v.SoThe == cardNumber);

                if (visaCardData == null)
                {
                    // Visa card does not exist
                    return Json(new { success = false, message = "Số thẻ không tồn tại!" });
                }

                // Validate other information
                if (visaCardData.ThoiHanThang != month)
                {
                    // Visa card information does not match
                    return Json(new { success = false, message = "Thời hạn tháng không hợp lệ!" });
                }

                // Validate other information
                if (visaCardData.ThoiHanNam != year)
                {
                    // Visa card information does not match
                    return Json(new { success = false, message = "Thời hạn năm không hợp lệ!" });
                }

                // Validate other information
                if (visaCardData.CVV != cvv)
                {
                    // Visa card information does not match
                    return Json(new { success = false, message = "CVV không hợp lệ!" });
                }
                if (visaCardData2 == null)
                {
                    string confirmationCode = GenerateConfirmationCode();

                    // Check if phoneNumber contains "@"
                    if (phoneNumber.Contains("@"))
                    {
                        bool sentSuccessfully = SendConfirmationCodeByEmail(phoneNumber, confirmationCode);

                        if (!sentSuccessfully)
                        {
                            return Json(new { success = false, message = "Failed to send confirmation code via email" });
                        }
                    }
                    else
                    {
                        bool sentSuccessfully = SendConfirmationCodeBySMS(phoneNumber, confirmationCode);

                        if (!sentSuccessfully)
                        {
                            return Json(new { success = false, message = "Failed to send confirmation code via SMS" });
                        }
                    }

                    // Store the confirmation code in Session
                    Session["ConfirmationCode"] = confirmationCode;

                    return Json(new { success = true, message = "Gửi mã OTP!", confirmationCode });
                }
                else
                {
                    // Visa card exists in TaiKhoanNganHangTaiXes, return error message
                    return Json(new { success = false, message = "Thẻ đã được sử dụng!" });
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return Json(new { success = false, message = "Đã xảy ra lỗi trong khi xử lý yêu cầu của bạn." });
            }
        }



        [HttpPost]
        public JsonResult InsertCard(string phoneNumber, string cardNumber, string cardHolder, int month, int year, int cvv)
        {
            try
            {
                // Insert the record into TaiKhanNganHangTaiXe table
                var visaCardData = db.TheVisas.SingleOrDefault(v => v.SoThe == cardNumber);

                
                var taiKhoanNganHang = new TaiKhoanNganHangTaiXe
                {
                    SoDienThoai = phoneNumber,
                    SoThe = cardNumber,
                    ChuThe = visaCardData.ChuThe, // Automatically populate ChuThe from TheVisa
                    ThoiHanThang = month,
                    ThoiHanNam = year,
                    CVV = cvv,
                    SoDu = (decimal)visaCardData.SoDu // Populate SoDu from TheVisa
                };

                db.TaiKhoanNganHangTaiXes.Add(taiKhoanNganHang);
                db.SaveChanges();

                return Json(new { success = true, message = $"Thẻ đăng ký thành công! Số dư hiện tại: {visaCardData.SoDu}" + " VND" });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return Json(new { success = false, message = "Đã xảy ra lỗi trong khi xử lý yêu cầu của bạn." });
            }
        }

        [HttpPost]
        public JsonResult UnlinkCard(string phoneNumber, string cardNumber)
        {
            try
            {
                // Ensure cardNumber is not null or empty
                if (string.IsNullOrEmpty(cardNumber))
                {
                    return Json(new { success = false, message = "Invalid card number." });
                }

                // Find the card in the database by cardNumber
                var taiKhoanNganHangTaiXe = db.TaiKhoanNganHangTaiXes.FirstOrDefault(t => t.SoThe == cardNumber);

                // Check if the card exists
                if (taiKhoanNganHangTaiXe == null)
                {
                    return Json(new { success = false, message = "Card not found." });
                }

                string confirmationCode = GenerateConfirmationCode();

                // Check if phoneNumber contains "@"
                if (phoneNumber.Contains("@"))
                {
                    bool sentSuccessfully = SendConfirmationCodeByEmail(phoneNumber, confirmationCode);

                    if (!sentSuccessfully)
                    {
                        return Json(new { success = false, message = "Failed to send confirmation code via email" });
                    }
                }
                else
                {
                    bool sentSuccessfully = SendConfirmationCodeBySMS(phoneNumber, confirmationCode);

                    if (!sentSuccessfully)
                    {
                        return Json(new { success = false, message = "Failed to send confirmation code via SMS" });
                    }
                }

                // Store the confirmation code in Session
                Session["ConfirmationCode"] = confirmationCode;

                return Json(new { success = true, message = "Gửi mã OTP!", confirmationCode });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Json(new { success = false, message = "Không thể hủy liên kết thẻ." });
            }
        }

        [HttpPost]
        public JsonResult UnlinkCard2(string phoneNumber, string cardNumber)
        {
            try
            {
                // Ensure cardNumber is not null or empty
                if (string.IsNullOrEmpty(cardNumber))
                {
                    return Json(new { success = false, message = "Invalid card number." });
                }

                // Find the card in the database by cardNumber
                var taiKhoanNganHangTaiXe = db.TaiKhoanNganHangTaiXes.FirstOrDefault(t => t.SoThe == cardNumber);

                // Find the card in the database by cardNumber
                var sodienthoai = db.TaiKhoanNganHangTaiXes.FirstOrDefault(t => t.SoDienThoai == phoneNumber);

                // Check if the card exists
                if (taiKhoanNganHangTaiXe == null)
                {
                    return Json(new { success = false, message = "Card not found." });
                }

                // Remove the card from the database
                db.TaiKhoanNganHangTaiXes.Remove(taiKhoanNganHangTaiXe);
                db.SaveChanges();


                return Json(new { success = true, message = "Thẻ đã được hủy liên kết." });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Json(new { success = false, message = "Không thể hủy liên kết thẻ." });
            }
        }



        [HttpPost]
        public JsonResult SendConfirmationCode(string email)
        {
            List<string> errorMessages = new List<string>();

            // Check if email or phone number was provided
            if (string.IsNullOrWhiteSpace(email))
            {
                errorMessages.Add("Email or phone number is required");
                return Json(new { success = false, message = "Email or phone number is required" });
            }

            string confirmationCode = GenerateConfirmationCode();

            if (email.Contains("@"))
            {
                // Email confirmation
                if (!SendConfirmationCodeByEmail(email, confirmationCode))
                {
                    errorMessages.Add("Failed to send confirmation code via email");
                    return Json(new { success = false, message = "Failed to send confirmation code via email" });
                }
            }

            if (errorMessages.Count > 0)
            {
                return Json(new { success = false, message = string.Join(", ", errorMessages) });
            }

            Session["SoDienThoai"] = email;
            TempData["ConfirmationCode"] = confirmationCode;

            return Json(new { success = true, message = "Confirmation code sent successfully" });
        }


        [HttpPost]
        public JsonResult EnterConfirmationCode(string confirmationCode)
        {
            // Check if confirmation code is correct
            if (string.IsNullOrWhiteSpace(confirmationCode))
            {
                return Json(new { success = false, message = "Confirmation code is required" });
            }

            // Ensure the comparison is case-sensitive and trim any extra spaces
            if (CheckConfirmationCode(confirmationCode.Trim()))
            {
                // The confirmation code is correct
                return Json(new { success = true, message = "Confirmation code is valid" });
            }
            else
            {
                // The confirmation code is incorrect
                return Json(new { success = false, message = "Invalid confirmation code" });
            }
        }



        private bool SendConfirmationCodeByEmail(string email, string confirmationCode)
        {
            try
            {
                // Replace the credentials and SMTP server with your own
                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("dangquy360@gmail.com", "vvgtfqfpltnesshc"),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("yourEmailAddress@gmail.com"),
                    Subject = "Visa card registration confirmation code",
                    Body = "Your confirmation code is: " + confirmationCode,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                smtpClient.Send(mailMessage);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool SendConfirmationCodeBySMS(string phoneNumber, string confirmationCode)
        {
            try
            {
                // Send confirmation code via SMS using Nexmo API
                var client = new Client(creds: new Nexmo.Api.Request.Credentials
                {
                    ApiKey = "85afba4e",
                    ApiSecret = "kQsuTG22VRLrgAx3"
                });
                var response = client.SMS.Send(request: new SMS.SMSRequest
                {
                    from = "Vonage APIs",
                    to = phoneNumber,
                    text = "Your confirmation code is: " + confirmationCode
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool CheckConfirmationCode(string confirmationCode)
        {
            // Check if confirmation code is correct
            var tempConfirmationCode = Session["ConfirmationCode"] as string;
            return !string.IsNullOrEmpty(tempConfirmationCode) && confirmationCode == tempConfirmationCode;
        }



        private string GenerateConfirmationCode()
        {
            // Generate random confirmation code
            Random random = new Random();
            return random.Next(10000, 99999).ToString();
        }

        [HttpGet]
        public ActionResult LichSuGiaoDich()
        {
            if (Session["SoDienThoai"] != null)
            {
                string phoneNumber = Session["SoDienThoai"].ToString();

                var model = new MultipleData();

                // Populate the TaiXes property
                model.taiXes = (from k in db.TaiXes
                                join t in db.TaiKhoans on k.SoDienThoai equals t.SoDienThoai
                                where (k.SoDienThoai == phoneNumber)
                                select k).ToList();

                // Populate the TaiXes property
                model.lichSuGiaoDichs = (from k in db.LichSuGiaoDiches
                                         join t in db.TaiXes on k.SoDienThoai equals t.SoDienThoai
                                         where (k.SoDienThoai == phoneNumber)
                                         select k)
                                         .OrderByDescending(k => k.MaGiaoDich)
                                         .ToList();
                // Add additional properties if needed

                return View(model);
            }
            else
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
        }

        [HttpPost]
        public JsonResult DisableAccount(string phoneNumber)
        {
            try
            {
                // Assuming db is your DbContext with a navigation property from TaiXe to TrangThaiHoatDong
                var TrangThai = db.QuyenTaiKhoans.FirstOrDefault(t => t.SoDienThoai == phoneNumber);

                if (TrangThai.SoDienThoai != null)
                {
                    // Toggle the status
                    TrangThai.MaQuyen = 4;

                    // Save changes to the database
                    db.SaveChanges();

                    // Return the updated status
                    return Json(new { success = true, status = TrangThai.MaQuyen });
                }

                return Json(new { success = false, message = "Driver status not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public JsonResult EnableAccount(string phoneNumber)
        {
            try
            {
                // Assuming db is your DbContext with a navigation property from TaiXe to TrangThaiHoatDong
                var TrangThai = db.QuyenTaiKhoans.FirstOrDefault(t => t.SoDienThoai == phoneNumber);

                if (TrangThai.SoDienThoai != null)
                {
                    // Toggle the status
                    TrangThai.MaQuyen = 2;

                    // Save changes to the database
                    db.SaveChanges();

                    // Return the updated status
                    return Json(new { success = true, status = TrangThai.MaQuyen });
                }

                return Json(new { success = false, message = "Driver status not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}