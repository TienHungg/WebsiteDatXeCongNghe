using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebsiteDatXeCongNghe.Models;

namespace WebsiteDatXeCongNghe.ViewModel
{
    public class DoiMatKhauViewModel: DichVuDatXeWebsite3Entities
    {
        //public int MaTK { get; set; }
        //public string SoDienThoai { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập mật khẩu hiện tại")]
		[DataType(DataType.Password)]
        [Display(Name = "OldPassword")]
        public string MatKhauHienTai { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập mật khẩu mới")]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "NewPassword")]
		public string MatKhauMoi { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "ConfirmNewPassword")]
		[Compare("MatKhauMoi", ErrorMessage = "The new password and confirmation password do not match.")]
		public string NhapLaiMatKhauMoi { get; set; }

    }
}