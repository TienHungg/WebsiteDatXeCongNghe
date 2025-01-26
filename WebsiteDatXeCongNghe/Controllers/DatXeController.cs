using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Twilio.Types;
using WebsiteDatXeCongNghe.Models;
using Nexmo.Api;
using Twilio.Types;
using Client = Nexmo.Api.Client;
using System.IO;

namespace WebsiteDatXeCongNghe.Controllers
{
    public class DatXeController : Controller
    {
        private DichVuDatXeWebsite3Entities db = new DichVuDatXeWebsite3Entities();
        // GET: DatXe
        public ActionResult ThongTinDatXe(ThongTinDatXe dx)
        {
            string a = Session["SoDienThoai"].ToString();
            var query = (from d in db.ThongTinDatXes
                         join t in db.KhachHangs on d.SoDienThoai equals t.SoDienThoai
                         where (d.SoDienThoai == a)
                         select d).ToList();
            return View(query);
        }
        public ActionResult GoogleMap()
        {            
            if (Session["SoDienThoai"] != null)
            {
                string a = Session["SoDienThoai"].ToString();
                var query = (from k in db.KhachHangs
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
        [HttpPost]
        public ActionResult GoogleMapThongTinDatXe(string distance, string duration, string totalFare, string location1, string location2, string type, string phone, string payment, DateTime DateTime, bool Status, string PassengerImage, string PassengerName, string PassengerEmail)
        {
            ThongTinDatXe dx = new ThongTinDatXe();
            dx.SoKm = distance;
            dx.ThoiGian = duration;
            dx.ThanhTien = totalFare;
            dx.DiemDon = location1;
            dx.DiemDen = location2;
            dx.LoaiXe = type;
            dx.SoDienThoai = phone;
            dx.HinhThucThanhToan = payment;
            dx.NgayGioDat = DateTime;
            dx.TrangThai = Status;
            dx.PassengerImage = PassengerImage;
            dx.PassengerName = PassengerName;
            dx.PassengerEmail = PassengerEmail;
            db.ThongTinDatXes.Add(dx);
            db.SaveChanges();
            // Return the MaDatXe value as a JSON response
            return Json(new { MaDatXe = dx.MaDatXe });
        }
        public ActionResult GoogleMap2()
        {
            if (Session["SoDienThoai"] != null)
            {
                string a = Session["SoDienThoai"].ToString();
                var query = (from k in db.KhachHangs
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
        [HttpPost]
        public ActionResult GoogleMap2(string distance, string duration, string totalFare, string location1, string location2, string type, string phone, string payment, DateTime DateTime)
        {
            ThongTinDatXe dx = new ThongTinDatXe();
            dx.SoKm = distance;
            dx.ThoiGian = duration;
            dx.ThanhTien = totalFare;
            dx.DiemDon = location1;
            dx.DiemDen = location2;
            dx.LoaiXe = type;
            dx.SoDienThoai = phone;
            dx.HinhThucThanhToan = payment;
            dx.NgayGioDat = DateTime;
            db.ThongTinDatXes.Add(dx);
            db.SaveChanges();
            return View();
        }
        //[HttpPost]
        //public JsonResult XoaThongTinDatXe(int MaDatXe)
        //{
        //    bool result = false;
        //    ThongTinDatXe dx = db.ThongTinDatXes.SingleOrDefault(x => x.MaDatXe == MaDatXe);
        //    if (dx != null)
        //    {
        //        db.ThongTinDatXes.Remove(dx);
        //        db.SaveChanges();
        //        result = true;
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        public ActionResult XoaThongTinDatXe(int id)
        {
            ThongTinDatXe txdx = db.ThongTinDatXes.Find(id);
            db.ThongTinDatXes.Remove(txdx);
            db.SaveChanges();
            return RedirectToAction("ThongTinDatXe");
        }
        public ActionResult NhanChuyenXe()
        {
            string a = Session["SoDienThoai"].ToString();
            var query = (from k in db.KhachHangs
                         join t in db.TaiKhoans on k.SoDienThoai equals t.SoDienThoai
                         where (k.SoDienThoai == a)
                         select k).ToList();
            return View(query);
        }       

        public ActionResult KhachDuocDon()
        {
            string a = Session["SoDienThoai"].ToString();
            var query = (from k in db.KhachHangs
                         join t in db.TaiKhoans on k.SoDienThoai equals t.SoDienThoai
                         where (k.SoDienThoai == a)
                         select k).ToList();
            return View(query);
        }
        public ActionResult KhachDuocTra()
        {
            string a = Session["SoDienThoai"].ToString();
            var query = (from k in db.KhachHangs
                         join t in db.TaiKhoans on k.SoDienThoai equals t.SoDienThoai
                         where (k.SoDienThoai == a)
                         select k).ToList();
            return View(query);
        }
        public ActionResult DanhGiaTaiXe()
        {
            string a = Session["SoDienThoai"].ToString();
            var query = (from k in db.KhachHangs
                         join t in db.TaiKhoans on k.SoDienThoai equals t.SoDienThoai
                         where (k.SoDienThoai == a)
                         select k).ToList();
            return View(query);
        }
        //[HttpPost]
        //public ActionResult DanhGiaTaiXe(string PassengerPhone, string DriverPhone, int value_rate, string content, DateTime date_rate)
        //{
        //    DanhGiaTaiXe dgtx = new DanhGiaTaiXe();
        //    dgtx.KHDanhGia = PassengerPhone;
        //    dgtx.TKTaiXe = DriverPhone;
        //    dgtx.MucDo = value_rate;
        //    dgtx.BinhLuan = content;
        //    dgtx.NgayDanhGia = DateTime.Now;
        //    db.DanhGiaTaiXes.Add(dgtx);
        //    db.SaveChanges();
        //    return RedirectToAction("GoogleMap");
        //}

        [HttpPost]
        public ActionResult DanhGiaTaiXe(string PassengerPhone, string DriverPhone, int value_rate, string content, DateTime date_rate)
        {
            try
            {
                // Create a new review object
                DanhGiaTaiXe dgtx = new DanhGiaTaiXe();
                dgtx.KHDanhGia = PassengerPhone;
                dgtx.TKTaiXe = DriverPhone;
                dgtx.MucDo = value_rate;
                dgtx.BinhLuan = content;
                dgtx.NgayDanhGia = DateTime.Now;

                // Add the review to the database
                db.DanhGiaTaiXes.Add(dgtx);
                db.SaveChanges();

                // Update the DiemUyTin in the TaiXe table based on the rating value
                var driver = db.TaiXes.SingleOrDefault(x => x.SoDienThoai == DriverPhone);

                if (driver != null)
                {
                    decimal diemUyTinToAdd = 0.0M;

                    // Determine the DiemUyTin to add based on the value_rate
                    if (value_rate == 3)
                    {
                        diemUyTinToAdd = 0.2M;
                    }
                    else if (value_rate == 4)
                    {
                        diemUyTinToAdd = 0.3M;
                    }
                    else if (value_rate == 5)
                    {
                        diemUyTinToAdd = 0.4M;
                    }

                    // Update DiemUyTin in TaiXe table
                    driver.DiemUyTin = driver.DiemUyTin + diemUyTinToAdd;
                    db.SaveChanges();
                }

                // Redirect to the appropriate page
                return RedirectToAction("GoogleMap");
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, etc.
                ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
                return View("Error");
            }
        }


        [HttpPost]
        public ActionResult SendPassData(string PhoneNumber, string DriverLocations, string DriverName, string DriverLicense, string txtLocationPickup, string txtLocationDrop, string result, string totalFare, string phoneNumberr, string distance, string duration, string type, string dated, /*string DriverRate,*/ string DriverImage, string PassImager, string PassNamer)
        {
            Session["PhoneNumber"] = PhoneNumber;
            Session["DriverLocations"] = DriverLocations;
            Session["DriverName"] = DriverName;
            Session["DriverLicense"] = DriverLicense;
            Session["txtLocationPickup"] = HttpUtility.HtmlEncode(txtLocationPickup);
            Session["txtLocationDrop"] = HttpUtility.HtmlEncode(txtLocationDrop);
            Session["result"] = result;
            Session["totalFare"] = totalFare;
            Session["phoneNumberr"] = phoneNumberr;
            Session["distance"] = distance;
            Session["duration"] = duration;
            Session["type"] = type;
            Session["dated"] = dated;
            //Session["DriverRate"] = DriverRate;
            Session["DriverImage"] = DriverImage;
            Session["PassImager"] = PassImager;
            Session["PassNamer"] = PassNamer;
            ViewBag.PickupLocation = Session["txtLocationPickup"];
            ViewBag.DropLocation = Session["txtLocationDrop"];
            return Json(new { success = true }); // Return a JSON response with the driver's image and rating
        }

        [HttpPost]
        public JsonResult GetVisaCardDetails(string phoneNumber)
        {
            try
            {
                // Fetch the data for the selected phone number
                var cardDetails = db.TaiKhoanNganHangs
                    .Where(t => t.SoDienThoai == phoneNumber)
                    .OrderBy(t => t.MaTaiKhoanNH) // Order by MaTaiKhoanNH
                    .Select(t => new { t.MaTaiKhoanNH, t.SoThe, t.ChuThe, t.ThoiHanThang, t.ThoiHanNam, t.CVV, t.SoDu })
                    .ToList();

                return Json(new { success = true, cardDetails });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);

                // Return an error response
                return Json(new { success = false, message = "Error fetching card details" });
            }
        }


        [HttpPost]
        public ActionResult ThanhToan(decimal amount, string PhoneNumber, string SoThe)
        {
            try
            {
                var Kh = db.KhachHangs.FirstOrDefault(t => t.SoDienThoai == PhoneNumber);
                var taiKhoan = db.TaiKhoanNganHangs.FirstOrDefault(t => t.SoThe == SoThe);
                var visaAccount = db.TheVisas.FirstOrDefault(v => v.SoThe == SoThe);
                // Validate the amount and input parameters if needed
                
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

                if (Kh == null)
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
        public ActionResult ThanhToan2(decimal amount, string PhoneNumber, string SoThe)
        {
            try
            {
                // Check if the TaiXe exists
                var KH = db.TaiXes.FirstOrDefault(t => t.SoDienThoai == PhoneNumber);
                // Check if the TaiKhoanNganHang exists
                var taiKhoan = db.TaiKhoanNganHangs.FirstOrDefault(t => t.SoThe == SoThe);

                // Check if the amount is greater than the balance in TheVisa
                var visaAccount = db.TheVisas.FirstOrDefault(v => v.SoThe == SoThe);

                
                var formattedBalanceCard = taiKhoan.SoDu.ToString("F3");
                

                // Save changes to the database
                db.SaveChanges();

                // Return the updated data in the response
                return Json(new
                {
                    success = true,
                    message = "Thanh toán chuyến xe ",
                    balanceCard = formattedBalanceCard
                });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public ActionResult Refund(decimal amount, string PhoneNumber, string SoThe)
        {
            try
            {
                // Check if the TaiKhoanNganHang exists
                var taiKhoan = db.TaiKhoanNganHangs.FirstOrDefault(t => t.SoThe == SoThe);
                // Check if the amount is greater than the balance in TheVisa
                var visaAccount = db.TheVisas.FirstOrDefault(v => v.SoThe == SoThe);


             

                // Save changes to the database
                db.SaveChanges();

                //// Return the updated data in the response
                //var formattedBalanceCard = taiKhoan.SoDu.ToString("F3");
                return Json(new
                {
                    success = true,
                    message = "Hoàn tiền thành công.",
                    //balanceCard = formattedBalanceCard
                });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public ActionResult SendSoThe(string CardId4, string CardHolder, string ExpirationMonth, string ExpirationYear, string CVV)
        {
            Session["CardId4"] = CardId4;            
            Session["CardHolder"] = CardHolder;            
            Session["ExpirationMonth"] = ExpirationMonth;            
            Session["ExpirationYear"] = ExpirationYear;            
            Session["CVV"] = CVV;            
            return Json(new { success = true });
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

    }
}