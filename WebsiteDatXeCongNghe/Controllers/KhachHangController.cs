using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteDatXeCongNghe.Models;
using WebsiteDatXeCongNghe.ViewModel.MultipleData;
using Nexmo.Api;

namespace WebsiteDatXeCongNghe.Controllers
{
    public class KhachHangController : Controller
    {
        // GET: KhachHang
        DichVuDatXeWebsite3Entities db = new DichVuDatXeWebsite3Entities();
        public ActionResult ThongTinKH(KhachHang kh)
        {
            string a = Session["SoDienThoai"].ToString();
            var query = (from k in db.KhachHangs
                         join t in db.TaiKhoans on k.SoDienThoai equals t.SoDienThoai
                         where (k.SoDienThoai == a)
                         select k).ToList();
            return View(query);
        }
        public ActionResult ThemMoiKH()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemMoiKH(KhachHang kh, HttpPostedFileBase HinhAnhUpload)
        {
            DichVuDatXeWebsite3Entities db = new DichVuDatXeWebsite3Entities();
            //Add new
            db.KhachHangs.Add(kh);
            //Save
            db.SaveChanges();
            if (HinhAnhUpload != null && HinhAnhUpload.ContentLength > 0)
            {
                int id = int.Parse(db.KhachHangs.ToList().Last().MaKH.ToString());
                string _FileName = "";
                int index = HinhAnhUpload.FileName.IndexOf('.');
                _FileName = "kh" + id.ToString() + "." + HinhAnhUpload.FileName.Substring(index + 1);
                String _path = Path.Combine(Server.MapPath("~/image/KhachHang"), _FileName);
                HinhAnhUpload.SaveAs(_path);

                KhachHang k = db.KhachHangs.FirstOrDefault(x => x.MaKH == id);
                k.HinhAnh = _FileName;
                db.SaveChanges();
            }
            ViewBag.Message = kh.Ten + kh.SoDienThoai + " Đăng ký thành công ";
            return RedirectToAction("DangNhap","TaiKhoan");

        }
        public ActionResult SuaDoiKH(int? id)
        {
            if (id == null)
            {
                // Handle the case when 'id' is null
                return RedirectToAction("ThongTinKH");
            }

            var Khach = db.KhachHangs.FirstOrDefault(k => k.MaKH == id);
            if (Khach == null)
            {
                // Handle the case when 'Khach' is null
                return RedirectToAction("ThongTinKH");
            }

            var khachDTO = new KhachHangDTO
            {
                MaKH = Khach.MaKH,
                SoDienThoai = Khach.SoDienThoai,
                Ten = Khach.Ten,
                NgayThangNamSinh = Khach.NgayThangNamSinh,
                CCCD = Khach.CCCD,
                Email = Khach.Email,
                HinhAnh = Khach.HinhAnh
            };

            return Json(khachDTO, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult SuaDoiKH(KhachHang kh, HttpPostedFileBase HinhAnhUpload)
        {
            if (HinhAnhUpload != null)
            {
                string path = uploadimage(HinhAnhUpload);
                kh.HinhAnh = path;
            }

            if (ModelState.IsValid)
            {
                db.Entry(kh).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ThongTinKH");
            }

            return View(kh);
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
                        path = Path.Combine(Server.MapPath("~/image/KhachHang"), random + Path.GetFileName(file.FileName));
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
        public ActionResult KhuyenMai()
        {
            var list = db.KhuyenMais.ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult LienKetThe()
        {
            if (Session["SoDienThoai"] != null)
            {
                string phoneNumber = Session["SoDienThoai"].ToString();

                var model = new MultipleData();

                // Populate the KhachHangs property
                model.khachHangs = (from k in db.KhachHangs
                                join t in db.TaiKhoans on k.SoDienThoai equals t.SoDienThoai
                                where (k.SoDienThoai == phoneNumber)
                                select k).ToList();

                // Populate the KhachHangs property
                model.taiKhoanNganHangs = (from k in db.TaiKhoanNganHangs
                                                join t in db.KhachHangs on k.SoDienThoai equals t.SoDienThoai
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
        public ActionResult GetSoDu(int? maTaiKhoan)
        {
            if (maTaiKhoan.HasValue)
            {
                // Retrieve SoDu value based on the provided MaTaiKhoanNHTaiXe
                var soDu = db.TaiKhoanNganHangs
                    .Where(t => t.MaTaiKhoanNH == maTaiKhoan.Value)
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

                    var soDu = db.TaiKhoanNganHangs
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
                var soThe = db.TaiKhoanNganHangs
                    .Where(t => t.MaTaiKhoanNH == maTaiKhoan.Value)
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

                    var soThe = db.TaiKhoanNganHangs
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

        [HttpGet]
        public ActionResult GetChuThe(int? maTaiKhoan)
        {
            if (maTaiKhoan.HasValue)
            {
                // Retrieve SoDu value based on the provided MaTaiKhoanNHTaiXe
                var chuThe = db.TaiKhoanNganHangs
                    .Where(t => t.MaTaiKhoanNH == maTaiKhoan.Value)
                    .Select(t => t.ChuThe)
                    .FirstOrDefault();

                // Check if soThe is null, and return "0" if it is
                return Content(chuThe ?? "0");
            }
            else
            {
                // Retrieve overall SoDu value based on the logged-in user's phone number
                if (Session["SoDienThoai"] != null)
                {
                    string phoneNumber = Session["SoDienThoai"].ToString();

                    var chuThe = db.TaiKhoanNganHangs
                        .Where(t => t.SoDienThoai == phoneNumber)
                        .Select(t => t.ChuThe)
                        .FirstOrDefault();

                    // Check if soThe is null, and return "0" if it is
                    return Content(chuThe ?? "0");
                }
                else
                {
                    return Content("0"); // Assuming a default value if the user is not logged in
                }
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
                var taiKhoanNganHangTaiXe = db.TaiKhoanNganHangs.FirstOrDefault(t => t.SoThe == cardNumber);

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
                var taiKhoanNganHangTaiXe = db.TaiKhoanNganHangs.FirstOrDefault(t => t.SoThe == cardNumber);

                // Find the card in the database by cardNumber
                var sodienthoai = db.TaiKhoanNganHangs.FirstOrDefault(t => t.SoDienThoai == phoneNumber);

                // Check if the card exists
                if (taiKhoanNganHangTaiXe == null)
                {
                    return Json(new { success = false, message = "Card not found." });
                }

                // Remove the card from the database
                db.TaiKhoanNganHangs.Remove(taiKhoanNganHangTaiXe);
                db.SaveChanges();


                return Json(new { success = true, message = "Thẻ đã được hủy liên kết." });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Json(new { success = false, message = "Không thể hủy liên kết thẻ." });
            }
        }

        [HttpGet]
        public ActionResult TaiKhoanNganHang()
        {
            if (Session["SoDienThoai"] != null)
            {
                string phoneNumber = Session["SoDienThoai"].ToString();

                var model = new MultipleData();

                // Populate the TaiXes property
                model.khachHangs = (from k in db.KhachHangs
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
        public JsonResult ValidateVisaCard(string phoneNumber, string cardNumber, string cardHolder, int month, int year, int cvv)
        {
            try
            {
                // Validate if the Visa card data exists in TheVisa table
                var visaCardData = db.TheVisas.SingleOrDefault(v => v.SoThe == cardNumber);
                var visaCardData2 = db.TaiKhoanNganHangs.SingleOrDefault(v => v.SoThe == cardNumber);

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


                var taiKhoanNganHang = new TaiKhoanNganHang
                {
                    SoDienThoai = phoneNumber,
                    SoThe = cardNumber,
                    ChuThe = visaCardData.ChuThe, // Automatically populate ChuThe from TheVisa
                    ThoiHanThang = month,
                    ThoiHanNam = year,
                    CVV = cvv,
                    SoDu = (decimal)visaCardData.SoDu // Populate SoDu from TheVisa
                };

                db.TaiKhoanNganHangs.Add(taiKhoanNganHang);
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