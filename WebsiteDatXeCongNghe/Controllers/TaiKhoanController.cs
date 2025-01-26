using WebsiteDatXeCongNghe.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebsiteDatXeCongNghe.Models;
using WebsiteDatXeCongNghe.ViewModel.TaiKhoanViewModel;
//using Twilio;
//using Twilio.Rest.Api.V2010.Account;
using Vonage;
using Vonage.Request;
//using Twilio.TwiML.Voice;
using Nexmo.Api;
using System.Net.Mail;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDatXeCongNghe.Controllers
{
    public class TaiKhoanController : Controller
    {
        private DichVuDatXeWebsite3Entities db = new DichVuDatXeWebsite3Entities();
        // GET: TaiKhoan
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(TaiKhoan tk)
        {
            using (DichVuDatXeWebsite3Entities db = new DichVuDatXeWebsite3Entities())
            {
                if (ModelState.IsValid)
                {
                    var check = db.TaiKhoans.FirstOrDefault(s => s.SoDienThoai == tk.SoDienThoai);
                    if (check == null)
                    {
                        db.TaiKhoans.Add(tk);
                        Session["SoDienThoai"] = tk.SoDienThoai.ToString();
                        db.SaveChanges();
                        // Use the value in db.QuyenTaiKhoans.Add or perform any other operations
                        var soDienThoai = Session["SoDienThoai"].ToString();
                        db.QuyenTaiKhoans.Add(new QuyenTaiKhoan { SoDienThoai = soDienThoai, MaQuyen = 3 });
                        db.SaveChanges();
                        return RedirectToAction("ThemMoiKH", "KhachHang");
                    }
                    else
                    {
                        ViewBag.Message = "Số điện thoại đã được đăng ký. Vui lòng sử dụng số điện thoại khác.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Message = "Đăng ký không thành công!";
                    return View();
                }
            }
        }

        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(TaiKhoan tk)
        {
            using (DichVuDatXeWebsite3Entities db = new DichVuDatXeWebsite3Entities())
            {
                var usr = db.TaiKhoans
                    .Join(db.QuyenTaiKhoans, t => t.SoDienThoai, qt => qt.SoDienThoai, (t, qt) => new { TaiKhoan = t, QuyenTaiKhoan = qt })
                    .Join(db.PhanQuyens, qtk => qtk.QuyenTaiKhoan.MaQuyen, pq => pq.MaQuyen, (qtk, pq) => new { TaiKhoan = qtk.TaiKhoan, Quyen = pq })
                    .Where(a => a.TaiKhoan.SoDienThoai.Equals(tk.SoDienThoai) && a.TaiKhoan.MatKhau.Equals(tk.MatKhau))
                    .FirstOrDefault();

                if (usr != null)
                {
                    Session["SoDienThoai"] = tk.SoDienThoai.ToString();
                    Session["MatKhau"] = tk.MatKhau.ToString();

                    // Redirect to appropriate page based on role
                    if (usr.Quyen.TenQuyen == "Super admin")
                    {
                        return RedirectToAction("BangDieuKhien", "QuanLy");
                    }
                    else if (usr.Quyen.TenQuyen == "User")
                    {
                        return RedirectToAction("Services");
                    }
                    else if (usr.Quyen.TenQuyen == "Driver")
                    {
                        return RedirectToAction("ThongTinTaiXe", "TaiXe");
                    }
                    else
                    {
                        return RedirectToAction("DangNhap", "TaiKhoan");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tên tài khoản hoặc mật khẩu sai!");
                }
            }

            return View();
        }
        //[HttpPost]
        //public ActionResult DangNhap(TaiKhoan tk)
        //{

        //    using (DichVuDatXeWebsite3Entities db = new DichVuDatXeWebsite3Entities())
        //    {
        //        var usr = db.TaiKhoans.Where(a => a.SoDienThoai.Equals(tk.SoDienThoai) && a.MatKhau.Equals(tk.MatKhau)).FirstOrDefault();
        //        if (usr != null)
        //        {
        //            Session["SoDienThoai"] = tk.SoDienThoai.ToString();
        //            Session["MatKhau"] = tk.MatKhau.ToString();

        //            return RedirectToAction("Services");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", " Tên tài khoản hoặc mật khẩu sai! ");
        //        }
        //    }
        //    return View();
        //}
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
        public ActionResult DangXuat()
        {
            Session.Abandon();//remove session
            return RedirectToAction("DangNhap");
        }
        public ActionResult Services()
        {
            if (Session["SoDienThoai"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("DangNhap");
            }
        }
        public ActionResult DoiMatKhau()
        {
            //var matkhau = new TaiKhoan();
            //matkhau.MatKhau = Session["MatKhau"].ToString();
            //var viewModel = new DoiMatKhauViewModel { MatKhauHienTai = matkhau.MatKhau };
            // Kiểm tra xem session MatKhau đã được lưu trữ hay chưa
            if (Session["SoDienThoai"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            return View(new DoiMatKhauViewModel());
        }


        [HttpPost]
        public ActionResult DoiMatKhau(DoiMatKhauViewModel model)
        {
            if (ModelState.IsValid)
            {
                //EwxsBVRi1LQS932NdL-hsvY1Y1tZqPwQ21doUy1a
                // Lấy thông tin tài khoản từ Session
                string soDienThoai = Session["SoDienThoai"].ToString();
                TaiKhoan taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.SoDienThoai == soDienThoai);

                // Kiểm tra mật khẩu cũ
                if (taiKhoan.MatKhau != model.MatKhauHienTai)
                {
                    ModelState.AddModelError("", "Mật khẩu cũ không đúng.");
                    return View("DoiMatKhau");
                }



                // Cập nhật mật khẩu mới
                taiKhoan.MatKhau = model.MatKhauMoi;
                model.NhapLaiMatKhauMoi = taiKhoan.MatKhau;
                db.Entry(taiKhoan).State = EntityState.Modified;
                // Save changes to the database
                db.SaveChanges();



                // Đăng xuất và quay lại trang đăng nhập
                Session.Abandon();
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            return View("DoiMatKhau");
        }
        //public ActionResult LuaChonHinhThuc()
        //{
        //    return View();
        //}
        //// POST: PasswordChange/SelectOption
        //[HttpPost]
        //public ActionResult LuaChonHinhThuc(string selectedOption)
        //{
        //    // Check if email or phone number was selected
        //    if (selectedOption == "email")
        //    {
        //        // Send email confirmation code and display the view for entering it
        //        string confirmationCode = SendConfirmationCodeByEmail();
        //        ViewBag.ConfirmationCode = confirmationCode;
        //        return View("XacNhanOTP");
        //    }
        //    else if (selectedOption == "phone")
        //    {
        //        // Send SMS confirmation code and display the view for entering it
        //        string confirmationCode = SendConfirmationCodeBySMS();
        //        ViewBag.ConfirmationCode = confirmationCode;
        //        return View("XacNhanOTP");
        //    }
        //    else
        //    {
        //        // Invalid selection, display error message
        //        ViewBag.ErrorMessage = "Invalid selection";
        //        return View("Index");
        //    }
        //}
        //public ActionResult XacNhanOTP()
        //{
        //    return View();
        //}
        //// POST: PasswordChange/EnterConfirmationCode
        //[HttpPost]
        //public ActionResult XacNhanOTP(string confirmationCode)
        //{
        //    // Check if confirmation code is correct
        //    if (CheckConfirmationCode(confirmationCode))
        //    {
        //        // Display the view for entering new password
        //        return View("EnterNewPassword");
        //    }
        //    else
        //    {
        //        // Invalid confirmation code, display error message
        //        ViewBag.ErrorMessage = "Invalid confirmation code";
        //        return View();
        //    }
        //}
        //public ActionResult CapNhatQuenMatKhau()
        //{
        //    return View();
        //}
        //// POST: PasswordChange/UpdatePassword
        //[HttpPost]
        //public ActionResult CapNhatQuenMatKhau(QuenMatKhauViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Update user's password and display success message
        //        UpdateUserPassword(model.NewPassword);
        //        ViewBag.SuccessMessage = "Password updated successfully";
        //        return View("PasswordChangeSuccess");
        //    }
        //    else
        //    {
        //        // Invalid password, display error message and return to the view for entering new password
        //        ViewBag.ErrorMessage = "Invalid password";
        //        return View("EnterNewPassword");
        //    }
        //}

        //// Helper methods for sending confirmation code, checking it, and updating password
        //private string SendConfirmationCodeByEmail()
        //{
        //    // Code to send confirmation code via email
        //    return "1234"; // Dummy confirmation code for demonstration purposes
        //}

        //private string SendConfirmationCodeBySMS()
        //{
        //    // Code to send confirmation code via SMS
        //    return "5678"; // Dummy confirmation code for demonstration purposes
        //}

        //private bool CheckConfirmationCode(string confirmationCode)
        //{
        //    // Code to check if confirmation code is correct
        //    return confirmationCode == "1234"; // Dummy check for demonstration purposes
        //}

        //private void UpdateUserPassword(string newPassword)
        //{
        //    // Code to update user's password
        //}
        ////public ActionResult SendSMS(string phoneNumber, string smsCode)
        ////{
        ////    // Your Twilio account SID and auth token
        ////    const string accountSid = "YOUR_ACCOUNT_SID";
        ////    const string authToken = "YOUR_AUTH_TOKEN";

        ////    // Initialize the Twilio client with your account SID and auth token
        ////    TwilioClient.Init(accountSid, authToken);

        ////    // Send the SMS message with the code to the user's phone number
        ////    var message = MessageResource.Create(
        ////        body: $"Your verification code is {smsCode}",
        ////        from: new Twilio.Types.PhoneNumber("YOUR_TWILIO_PHONE_NUMBER"),
        ////        to: new Twilio.Types.PhoneNumber(phoneNumber)
        ////    );

        ////    return View();
        ////}
        //public ActionResult SendSMS(string phoneNumber, string smsCode)
        //{
        //    // Your Nexmo API key and secret
        //    const string apiKey = "YOUR_API_KEY";
        //    const string apiSecret = "YOUR_API_SECRET";

        //    // Initialize the Nexmo client with your API key and secret
        //    var client = new Client(creds: new Nexmo.Api.Request.Credentials
        //    {
        //        ApiKey = apiKey,
        //        ApiSecret = apiSecret
        //    });

        //    // Send the SMS message with the code to the user's phone number
        //    var results = client.SMS.Send(request: new SMS.SMSRequest
        //    {
        //        from = "YOUR_NEXMO_PHONE_NUMBER",
        //        to = phoneNumber,
        //        text = $"Your verification code is {smsCode}"
        //    });

        //    return View();
        //}



        // GET: TaiKhoan
        public ActionResult LuaChonHinhThuc()
        {
            // Display the initial view for selecting email or phone number
            return View();
        }

        // POST: TaiKhoan/SelectOption
        [HttpPost]
        public ActionResult SelectOption(string selectedOption)
        {
            // Check if email or phone number was selected
            if (selectedOption == "email")
            {
                return View("EnterEmailOrPhone");
            }
            else if (selectedOption == "phone")
            {
                return View("EnterEmailOrPhone");
            }
            else
            {
                // Invalid selection, display error message
                ViewBag.ErrorMessage = "Invalid selection";
                return View("LuaChonHinhThuc");
            }
        }
        //public ActionResult EnterEmailOrPhone()
        //{
        //    return View();
        //}
        public ActionResult SendConfirmationCode()
        {
            return View();
        }
        // POST: TaiKhoan/SendConfirmationCode

        //public ActionResult SendConfirmationCode(string emailOrPhone)
        //{
        //    TaiKhoan tk = new TaiKhoan();
        //    var check = db.TaiKhoans.FirstOrDefault(s => s.SoDienThoai == tk.SoDienThoai);
        //    // Check if email or phone number was provided
        //    if (string.IsNullOrWhiteSpace(emailOrPhone))
        //    {
        //        ViewBag.ErrorMessage = "Email or phone number is required";
        //        return View("EnterEmailOrPhone");
        //    }

        //    string confirmationCode = GenerateConfirmationCode();

        //    if (emailOrPhone.Contains("@"))
        //    {
        //        // Email confirmation
        //        if (!SendConfirmationCodeByEmail(emailOrPhone, confirmationCode))
        //        {
        //            TempData["SoDienThoai"] = emailOrPhone;
        //            if (TempData["SoDienThoai"] != check) 
        //            {
        //                ViewBag.ErrorMessage = "Failed to send not find!";
        //                ViewBag.ErrorMessage = "Failed to send confirmation code via email";
        //                return View("EnterEmailOrPhone");
        //            }                    
        //        }
        //    }
        //    else
        //    {
        //        // Phone confirmation
        //        if (!SendConfirmationCodeBySMS(emailOrPhone, confirmationCode))
        //        {
        //            TempData["SoDienThoai"] = emailOrPhone;
        //            if (TempData["SoDienThoai"] != check)
        //            {
        //                ViewBag.ErrorMessage = "Failed to send not find!";
        //                ViewBag.ErrorMessage = "Failed to send confirmation code via email";
        //                return View("EnterEmailOrPhone");
        //            }
        //        }
        //    }

        //    TempData["ConfirmationCode"] = confirmationCode;
        //    return View();

        //}
       
        [HttpPost]
        public ActionResult SendConfirmationCode(string emailOrPhone)
        {
            List<string> errorMessages = new List<string>();
            // Check if email or phone number was provided
            if (string.IsNullOrWhiteSpace(emailOrPhone))
            {
                errorMessages.Add("Email or phone number is required");
                return View("EnterEmailOrPhone");
            }

            string confirmationCode = GenerateConfirmationCode();

            TaiKhoan tk = db.TaiKhoans.FirstOrDefault(s => s.SoDienThoai == emailOrPhone);
            if (tk == null)
            {
                errorMessages.Add("The phone number is not exist!");
                return View("EnterEmailOrPhone");
            }

            if (emailOrPhone.Contains("@"))
            {
                // Email confirmation
                if (!SendConfirmationCodeByEmail(emailOrPhone, confirmationCode))
                {
                    errorMessages.Add("Failed to send confirmation code via email");
                    return View("EnterEmailOrPhone");
                }
            }
            else
            {
                // Phone confirmation
                if (!SendConfirmationCodeBySMS(emailOrPhone, confirmationCode))
                {
                    errorMessages.Add("Failed to send confirmation code via SMS");
                    return View("EnterEmailOrPhone");
                }
            }
            if (errorMessages.Count > 0)
            {
                ViewBag.ErrorMessages = errorMessages;
                return View("EnterEmailOrPhone");
            }
            Session["SoDienThoai"] = emailOrPhone;
            TempData["ConfirmationCode"] = confirmationCode;
            return View();
        }
        
        //public ActionResult SendConfirmationCode()
        //{
        //    return View();
        //}
        // POST: TaiKhoan/EnterConfirmationCode
        [HttpPost]
        public ActionResult EnterConfirmationCode(string confirmationCode)
        {
            // Check if confirmation code is correct
            if (string.IsNullOrWhiteSpace(confirmationCode))
            {
                ViewBag.ErrorMessage = "Confirmation code is required";
                return View("SendConfirmationCode");
            }

            if (CheckConfirmationCode(confirmationCode))
            {
                return View("EnterNewPassword");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid confirmation code";
                return View("SendConfirmationCode");
            }
        }
        // POST: TaiKhoan/VerifyConfirmationCode
        public ActionResult VerifyConfirmationCode()
        {
            return View();
        }
        [HttpPost]
        public ActionResult VerifyConfirmationCode(string confirmationCode)
        {
            // Check if confirmation code is correct
            if (string.IsNullOrWhiteSpace(confirmationCode))
            {
                ViewBag.ErrorMessage = "Confirmation code is required";
                return View("SendConfirmationCode");
            }
            

            if (CheckConfirmationCode(confirmationCode))
            {
              
                return RedirectToAction("EnterNewPassword");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid confirmation code";
                return View("SendConfirmationCode");
            }
        }

        public ActionResult EnterNewPassword()
        {           
            return View();
        }
        // POST: TaiKhoan/UpdatePassword
        [HttpPost]
        public ActionResult UpdatePassword(QuenMatKhauViewModel model)
        {
            if (ModelState.IsValid)
            {
                string soDienThoai = Session["SoDienThoai"].ToString();
                TaiKhoan taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.SoDienThoai == soDienThoai);
                taiKhoan.MatKhau = model.NewPassword;
                model.ConfirmNewPassword = taiKhoan.MatKhau;
                db.Entry(taiKhoan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DangNhap");
            }
            return View("EnterNewPassword");
        }
        
        private string GenerateConfirmationCode()
        {
            // Generate random confirmation code
            Random random = new Random();
            return random.Next(10000, 99999).ToString();
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        //private bool SendConfirmationCodeByEmail(string email, string confirmationCode)
        //{
        //    try
        //    {
        //        var smtpClient = new SmtpClient("smtp.gmail.com", 587)
        //        {
        //            Credentials = new NetworkCredential("dangquy360@gmail.com", "vvgtfqfpltnesshc"),
        //            EnableSsl = true
        //        };

        //        // Send confirmation code via email using SMTP client
        //        var message = new MailMessage();
        //        message.To.Add(new MailAddress(email));
        //        message.Subject = "Password Reset Confirmation Code";
        //        message.Body = "Your confirmation code is: " + confirmationCode;
        //        smtpClient.Send(message);

        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        //private bool SendConfirmationCodeByEmail(string email, string confirmationCode)
        //{
        //    try
        //    {
        //        // Send confirmation code via email using SMTP client
        //        var client = new SmtpClient();
        //        var message = new MailMessage();
        //        message.To.Add(new MailAddress(email));
        //        message.Subject = "Password Reset Confirmation Code";
        //        message.Body = "Your confirmation code is: " + confirmationCode;
        //        client.Send(message);
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
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
                    Subject = "Password Reset Confirmation Code",
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
                    ApiKey = "0629f7b4",
                    ApiSecret = "B2wANBPsNhSOgD9l"
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
        //private bool SendConfirmationCodeBySMS(string phoneNumber, string confirmationCode)
        //{
        //    try
        //    {
        //        // Send confirmation code via SMS using Twilio API
        //        var accountSid = "your_account_sid";
        //        var authToken = "your_auth_token";
        //        TwilioClient.Init(accountSid, authToken);
        //        var message = MessageResource.Create(
        //            body: "Your confirmation code is: " + confirmationCode,
        //            from: new Twilio.Types.PhoneNumber("your_twilio_phone_number"),
        //            to: new Twilio.Types.PhoneNumber(phoneNumber)
        //        );
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        //private bool CheckConfirmationCode(string confirmationCode)
        //{
        //    // Check if confirmation code is correct
        //    return confirmationCode == TempData["ConfirmationCode"].ToString();
        //}
        private bool CheckConfirmationCode(string confirmationCode)
        {
            // Check if confirmation code is correct
            var tempConfirmationCode = TempData["ConfirmationCode"] as string;
            return !string.IsNullOrEmpty(tempConfirmationCode) && confirmationCode == tempConfirmationCode;
        }

        private void UpdateUserPassword()
        {
            // Update user's password
        }
    }
}