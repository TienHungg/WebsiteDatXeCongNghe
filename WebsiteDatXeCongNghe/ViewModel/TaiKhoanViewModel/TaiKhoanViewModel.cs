using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DichVuDatXeCongNghe.ViewModel
{
    
    public class TaiKhoanViewModel
    {       
        [Key, Column(Order = 1)]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MaTK { get; set; }
        [Required(ErrorMessage ="Yêu cầu nhập số điện thoại!")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }
        [Required(ErrorMessage ="Yêu cầu nhập mật khẩu!")]
        [Display(Name ="Mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau{ get; set; }
        [Required(ErrorMessage ="Yêu cầu xác minh mật khẩu!")]
        [Display(Name ="Nhập lại mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("MatKhau",ErrorMessage ="Mật khẩu không khớp, yêu cầu nhập lại!")]
        public string NhapLaiMatKhau { get; set; }
        
    }
}