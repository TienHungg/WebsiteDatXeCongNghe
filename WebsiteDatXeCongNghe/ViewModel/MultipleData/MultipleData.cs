using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteDatXeCongNghe.Models;

namespace WebsiteDatXeCongNghe.ViewModel.MultipleData
{
    public class MultipleData
    {
        public IEnumerable<KhachHang> khachHangs { get; set; }
        public IEnumerable<ThongTinDatXe> thongTinDatXes { get; set; }
        public IEnumerable<TaiKhoan> taiKhoans { get; set; }
        public IEnumerable<TaiXe> taiXes { get; set; }
        public IEnumerable<DanhGiaUngDung> danhGias { get; set; }
        public IEnumerable<KhuyenMai> khuyenMais { get; set; }
        public IEnumerable<ThanhToan> thanhToans { get; set; }
        public IEnumerable<DanhGiaTaiXe> danhGiaTaiXes { get; set; }
        public IEnumerable<TaiXeNhanChuyenXe> taiXeNhanChuyenXes { get; set; }
        public IEnumerable<TaiKhoanNganHang> taiKhoanNganHangs { get; set; }
        public IEnumerable<QuyenTaiKhoan> quyenTaiKhoans { get; set; }
        public IEnumerable<PhanQuyen> phanQuyens { get; set; }
        public IEnumerable<TrangThaiHoatDong> trangThaiHoatDongs { get; set; }
        public IEnumerable<TaiKhoanNganHangTaiXe> taiKhoanNganHangTaiXes { get; set; }
        public IEnumerable<TheVisa> theVisas { get; set; }
        public IEnumerable<LichSuGiaoDich> lichSuGiaoDichs { get; set; }

    }
}