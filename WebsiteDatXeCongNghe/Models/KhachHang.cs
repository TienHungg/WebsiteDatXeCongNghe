//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebsiteDatXeCongNghe.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class KhachHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KhachHang()
        {
            this.DanhGiaTaiXes = new HashSet<DanhGiaTaiXe>();
            this.TaiKhoanNganHangs = new HashSet<TaiKhoanNganHang>();
            this.ThanhToans = new HashSet<ThanhToan>();
            this.ThongTinDatXes = new HashSet<ThongTinDatXe>();
        }
    
        public int MaKH { get; set; }
        public string SoDienThoai { get; set; }
        public string Ten { get; set; }
        public System.DateTime NgayThangNamSinh { get; set; }
        public string CCCD { get; set; }
        public string Email { get; set; }
        public string HinhAnh { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DanhGiaTaiXe> DanhGiaTaiXes { get; set; }
        public virtual DanhGiaUngDung DanhGiaUngDung { get; set; }
        public virtual TaiKhoan TaiKhoan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TaiKhoanNganHang> TaiKhoanNganHangs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThanhToan> ThanhToans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThongTinDatXe> ThongTinDatXes { get; set; }
    }
}
