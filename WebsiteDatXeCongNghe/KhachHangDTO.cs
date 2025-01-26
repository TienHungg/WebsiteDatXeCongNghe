using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteDatXeCongNghe
{
    public class KhachHangDTO
    {
        public int MaKH { get; set; }
        public string SoDienThoai { get; set; }
        public string Ten { get; set; }
        public DateTime NgayThangNamSinh { get; set; }
        public string CCCD { get; set; }
        public string Email { get; set; }
        public string HinhAnh { get; set; }
    }
}