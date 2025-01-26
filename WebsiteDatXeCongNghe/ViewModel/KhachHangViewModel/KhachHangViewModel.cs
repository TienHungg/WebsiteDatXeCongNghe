using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DichVuDatXeCongNghe.ViewModel
{
    public class KhachHangViewModel
    {
        private int MaKH { get; set; }
        private String SoDienThoai { get; set; }
        private String Ten { get; set; }
        private DateTime NgayThangNamSinh { get; set; }
        private String Email { get; set; }
        [DisplayName("Upload file")]
        private String HinhAnh { get; set; }
        public HttpPostedFileBase HinhAnhUpload { get; set; }

        [ForeignKey("SoDienThoai")]
        public TaiKhoanViewModel TaiKhoan { get; set; }
    }
}