using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteDatXeCongNghe
{
    public class TaiXeDTO
    {
        public int MaTX { get; set; }
        public string SoDienThoai { get; set; }
        public string Ten { get; set; }
        public System.DateTime NgayThangNamSinh { get; set; }
        public string Email { get; set; }
        public string BienSo { get; set; }
        public string CCCD { get; set; }
        public System.DateTime NgayDangKy { get; set; }
        public string HinhAnh { get; set; }
        public Nullable<decimal> MucDoDanhGiaTB { get; set; }
        public Nullable<decimal> DiemUyTin { get; set; }
        public string ViTri { get; set; }
    }
}