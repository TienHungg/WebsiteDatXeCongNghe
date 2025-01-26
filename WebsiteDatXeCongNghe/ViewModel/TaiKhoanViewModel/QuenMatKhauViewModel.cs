using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace WebsiteDatXeCongNghe.ViewModel.TaiKhoanViewModel
{
    public class QuenMatKhauViewModel
    {
        public string CommunicationMethod { get; set; }
        public string CommunicationValue { get; set; }
        public string OTPCode { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập mật khẩu mới")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword")]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmNewPassword")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}