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
    
    public partial class TaiXeNhanChuyenXe
    {
        public int MaNhanChuyen { get; set; }
        public string SoDienThoai { get; set; }
        public int MaDatXe { get; set; }
        public System.DateTime NgayNhan { get; set; }
        public Nullable<bool> TrangThai { get; set; }
        public string GhiChu { get; set; }
    
        public virtual TaiXe TaiXe { get; set; }
        public virtual ThongTinDatXe ThongTinDatXe { get; set; }
    }
}
