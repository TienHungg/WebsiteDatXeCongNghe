var tabs = document.querySelectorAll(".tabs_wrap ul li");
var KhachHang = document.querySelectorAll(".KhachHang");
var ThongTinDatXe = document.querySelectorAll(".ThongTinDatXe");
var TaiKhoan = document.querySelectorAll(".TaiKhoan");
var TaiXe = document.querySelectorAll(".TaiXe");
var DanhGiaUngDung = document.querySelectorAll(".DanhGiaUngDung");
var KhuyenMai = document.querySelectorAll(".KhuyenMai");
var ThanhToan = document.querySelectorAll(".ThanhToan");
var DanhGiaTaiXe = document.querySelectorAll(".DanhGiaTaiXe");
var TaiXeNhanChuyenXe = document.querySelectorAll(".TaiXeNhanChuyenXe");
var TaiKhoanNganHang = document.querySelectorAll(".TaiKhoanNganHang");
var QuyenTaiKhoan = document.querySelectorAll(".QuyenTaiKhoan");
var PhanQuyen = document.querySelectorAll(".PhanQuyen");
var all = document.querySelectorAll(".item_wrap");
const btnadd = document.querySelector(".js-hidden");

tabs.forEach((tab) => {
	tab.addEventListener("click", () => {
		tabs.forEach((tab) => {
			tab.classList.remove("active");
		})
		tab.classList.add("active");
		var tabval = tab.getAttribute("data-tabs");

		all.forEach((item) => {
			item.style.display = "none";
		})

		if (tabval == "KhachHang") {
			KhachHang.forEach((KhachHang) => {
				KhachHang.style.display = "block";
				btnadd.style.display = "none";
			})
		}
		else if (tabval == "ThongTinDatXe") {
			ThongTinDatXe.forEach((ThongTinDatXe) => {
				ThongTinDatXe.style.display = "block";
				btnadd.style.display = "none";
			})
		}
		else if (tabval == "TaiKhoan") {
			TaiKhoan.forEach((TaiKhoan) => {
				TaiKhoan.style.display = "block";
				btnadd.style.display = "block";
			})
		}
		else if (tabval == "TaiXe") {
			TaiXe.forEach((TaiXe) => {
				TaiXe.style.display = "block";
				btnadd.style.display = "none";
			})
		}
		else if (tabval == "DanhGiaUngDung") {
			DanhGiaUngDung.forEach((DanhGiaUngDung) => {
				DanhGiaUngDung.style.display = "block";
				btnadd.style.display = "none";
			})
		}
		else if (tabval == "KhuyenMai") {
			KhuyenMai.forEach((KhuyenMai) => {
				KhuyenMai.style.display = "block";
				btnadd.style.display = "none";
			})
		}
		else if (tabval == "ThanhToan") {
			ThanhToan.forEach((ThanhToan) => {
				ThanhToan.style.display = "block";
				btnadd.style.display = "none";
			})
		}
		else if (tabval == "DanhGiaTaiXe") {
			DanhGiaTaiXe.forEach((DanhGiaTaiXe) => {
				DanhGiaTaiXe.style.display = "block";
				btnadd.style.display = "none";
			})
		}
		else if (tabval == "TaiXeNhanChuyenXe") {
			TaiXeNhanChuyenXe.forEach((TaiXeNhanChuyenXe) => {
				TaiXeNhanChuyenXe.style.display = "block";
				btnadd.style.display = "none";
			})
		}
		else if (tabval == "TaiKhoanNganHang") {
			TaiKhoanNganHang.forEach((TaiKhoanNganHang) => {
				TaiKhoanNganHang.style.display = "block";
				btnadd.style.display = "none";
			})
		}
		else if (tabval == "QuyenTaiKhoan") {
			QuyenTaiKhoan.forEach((QuyenTaiKhoan) => {
				QuyenTaiKhoan.style.display = "block";
				btnadd.style.display = "none";
			})
		}
		else if (tabval == "PhanQuyen") {
			PhanQuyen.forEach((PhanQuyen) => {
				PhanQuyen.style.display = "block";
				btnadd.style.display = "none";
			})
		}
		else {
			all.forEach((item) => {
				item.style.display = "block";
			})
		}

	})
})