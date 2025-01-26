using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteDatXeCongNghe.Models;
using WebsiteDatXeCongNghe.ViewModel.MultipleData;

namespace WebsiteDatXeCongNghe.Controllers
{
    public class ThongKeController : Controller
    {
        private DichVuDatXeWebsite3Entities db = new DichVuDatXeWebsite3Entities();
        // GET: ThongKe
        //public ActionResult ThongKeDoanhThu()
        //{
        //    return View();
        //}        

        [HttpGet]
        public ActionResult ThongKeDoanhThu()
        {
            if (Session["SoDienThoai"] != null)
            {
                string phoneNumber = Session["SoDienThoai"].ToString();

                var mymodel = new MultipleData();

                // Populate the TaiXes property
                mymodel.taiXes = (from k in db.TaiXes
                                  join t in db.ThanhToans on k.SoDienThoai equals t.SoDienThoaiTX
                                  where (k.SoDienThoai == phoneNumber)
                                  select k).ToList();

                // Retrieve top 3 rides for the specific driver's phone number
                var top3Ride = db.TaiXeNhanChuyenXes
                    .Where(t => t.SoDienThoai == phoneNumber)
                    .OrderByDescending(t => t.MaNhanChuyen)
                    .Take(3)
                    .ToList();

                // Verify top3Ride contains data
                if (top3Ride.Count > 0)
                {
                    // Extract MaDatXe values from top3Ride
                    var maDatXes = top3Ride.Select(t => t.MaDatXe).ToList();

                    // Load ThongTinDatXes for corresponding MaDatXe in the top 3 rides
                    mymodel.thongTinDatXes = db.ThongTinDatXes.Where(tt => maDatXes.Contains(tt.MaDatXe)).ToList();

                    // Load ThanhToans
                    mymodel.taiXeNhanChuyenXes = top3Ride;
                }

                return View(mymodel);
            }
            else
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
        }


        [HttpGet]
        public ActionResult GetTotalDoanhThu()
        {
            // Calculate the total sum of TongTien and divide by 30%
            decimal? totalDoanhThu = db.ThanhToans.Sum(t => (decimal?)t.TongTien) * 0.3m;

            return Json(totalDoanhThu, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTotalDoanhThuNamNay()
        {
            // Calculate the total sum of TongTien for the year 2023 and multiply by 0.3
            DateTime startOfYear = new DateTime(2023, 1, 1);
            DateTime endOfYear = new DateTime(2023, 12, 31);

            decimal? totalDoanhThuNamNay = db.ThanhToans
                .Where(t => t.NgayThanhToan >= startOfYear && t.NgayThanhToan <= endOfYear)
                .Sum(t => (decimal?)t.TongTien) * 0.3m;

            return Json(totalDoanhThuNamNay, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTotalDoanhThuHomNay()
        {
            // Get the current date and time in Vietnam's timezone (UTC+7)
            DateTime nowInVietnam = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            // Calculate the total sum of TongTien for today
            DateTime todayStart = nowInVietnam.Date;
            DateTime todayEnd = todayStart.AddDays(1);

            decimal? totalDoanhThuHomNay = db.ThanhToans
                .Where(t => t.NgayThanhToan >= todayStart && t.NgayThanhToan < todayEnd)
                .Sum(t => (decimal?)t.TongTien) * 0.3m;

            return Json(totalDoanhThuHomNay ?? 0m, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetTotalBill()
        {
            // Count the total number of MaThanhToan
            int totalBill = db.ThanhToans.Count();

            return Json(totalBill, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadBarChart(int? selectedYear)
        {
            var mymodel = new MultipleData();

            // Filter ThanhToans based on the selected year
            var invoices = db.ThanhToans
                .Where(t => selectedYear == null || t.NgayThanhToan.Year == selectedYear)
                .ToList();

            // Group invoices by month and calculate the rounded sum of TongTien for each month
            var monthlyRevenue = invoices
                .GroupBy(t => t.NgayThanhToan.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    TotalRevenue = Math.Round((double)(group.Sum(t => t.TongTien) ?? 0) * 0.3, 3) // Rounded to 3 decimal places
                })
                .OrderBy(entry => entry.Month) // Keep ascending order of months
                .ToList();

            return Json(monthlyRevenue, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTotalDoanhThuByDriver(string driverPhone)
        {
            // Calculate the total sum of TongTien where SoDienThoaiTX matches the provided driverPhone
            decimal? totalDoanhThuByDriver = db.ThanhToans
                .Where(t => t.SoDienThoaiTX == driverPhone)
                .Sum(t => (decimal?)t.TongTien) * 0.7m;

            return Json(totalDoanhThuByDriver, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTotalDoanhThuNamNayByDriver(string driverPhone)
        {
            // Calculate the total sum of TongTien for the year 2023 where SoDienThoaiTX matches the provided driverPhone
            DateTime startOfYear = new DateTime(2024, 1, 1);
            DateTime endOfYear = new DateTime(2024, 12, 31);

            decimal? totalDoanhThuNamNayByDriver = db.ThanhToans
                .Where(t => t.SoDienThoaiTX == driverPhone && t.NgayThanhToan >= startOfYear && t.NgayThanhToan <= endOfYear)
                .Sum(t => (decimal?)t.TongTien) * 0.7m;

            return Json(totalDoanhThuNamNayByDriver, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTotalDoanhThuHomNayByDriver(string driverPhone)
        {
            // Get the current date and time in Vietnam's timezone (UTC+7)
            DateTime nowInVietnam = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            // Calculate the total sum of TongTien for today where SoDienThoaiTX matches the provided driverPhone
            DateTime todayStart = nowInVietnam.Date;
            DateTime todayEnd = todayStart.AddDays(1);

            decimal? totalDoanhThuHomNayByDriver = db.ThanhToans
                .Where(t => t.SoDienThoaiTX == driverPhone && t.NgayThanhToan >= todayStart && t.NgayThanhToan < todayEnd)
                .Sum(t => (decimal?)t.TongTien) * 0.7m;

            return Json(totalDoanhThuHomNayByDriver ?? 0m, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTotalBillByDriver(string driverPhone)
        {
            // Count the total number of MaThanhToan where SoDienThoaiTX matches the provided driverPhone
            int totalBillByDriver = db.ThanhToans.Count(t => t.SoDienThoaiTX == driverPhone);

            return Json(totalBillByDriver, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadBarChartByDriver(int? selectedYear, string driverPhone)
        {
            var mymodel = new MultipleData();

            // Filter ThanhToans based on the selected year and driverPhone
            var invoices = db.ThanhToans
                .Where(t => (selectedYear == null || t.NgayThanhToan.Year == selectedYear) && (driverPhone == null || t.SoDienThoaiTX == driverPhone))
                .ToList();

            // Group invoices by month and calculate the rounded sum of TongTien for each month
            var monthlyRevenue = invoices
                .GroupBy(t => t.NgayThanhToan.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    TotalRevenue = Math.Round((double)(group.Sum(t => t.TongTien) ?? 0) * 0.7, 3) // Rounded to 3 decimal places
                })
                .OrderBy(entry => entry.Month) // Keep ascending order of months
                .ToList();

            return Json(monthlyRevenue, JsonRequestBehavior.AllowGet);
        }



        public ActionResult ThongKeDoanhThu2()
        {
            return View();
        }
        public ActionResult GetRevenueData(string period, string selectedDate)
        {
            List<dynamic> revenueData = new List<dynamic>();

            DateTime? date = null;
            if (!string.IsNullOrEmpty(selectedDate))
            {
                date = DateTime.ParseExact(selectedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            if (period == "day")
            {
                revenueData = db.ThongTinDatXes
                    .Where(x => date == null || DbFunctions.TruncateTime(x.NgayGioDat) == date)
                    .GroupBy(x => DbFunctions.TruncateTime(x.NgayGioDat))
                    .ToList()
                    .Select(g => new
                    {
                        NgayGioDat = g.Key,
                        ThanhTien = g.Sum(x => decimal.Parse(x.ThanhTien.Replace(" VND", "").Replace(".", "")))
                    })
                    .ToList<dynamic>();
            }
            else if (period == "week")
            {
                revenueData = db.ThongTinDatXes
                    .Where(x => date == null || (x.NgayGioDat.Year == date.Value.Year && SqlFunctions.DatePart("wk", x.NgayGioDat) == SqlFunctions.DatePart("wk", date.Value)))
                    .GroupBy(x => new { Year = x.NgayGioDat.Year, Week = SqlFunctions.DatePart("wk", x.NgayGioDat) })
                    .ToList()
                    .Select(g => new
                    {
                        NgayGioDat = g.Select(x => x.NgayGioDat).FirstOrDefault(),
                        ThanhTien = g.Sum(x => decimal.Parse(x.ThanhTien.Replace(" VND", "").Replace(".", "")))
                    })
                    .ToList<dynamic>();
            }
            else if (period == "month")
            {
                revenueData = db.ThongTinDatXes
                    .Where(x => date == null || (x.NgayGioDat.Year == date.Value.Year && x.NgayGioDat.Month == date.Value.Month))
                    .GroupBy(x => new { Year = x.NgayGioDat.Year, Month = x.NgayGioDat.Month })
                    .ToList()
                    .Select(g => new
                    {
                        NgayGioDat = g.Select(x => x.NgayGioDat).FirstOrDefault(),
                        ThanhTien = g.Sum(x => decimal.Parse(x.ThanhTien.Replace(" VND", "").Replace(".", "")))
                    })
                    .ToList<dynamic>();
            }
            else if (period == "year")
            {
                revenueData = db.ThongTinDatXes
                    .Where(x => date == null || x.NgayGioDat.Year == date.Value.Year)
                    .GroupBy(x => x.NgayGioDat.Year)
                    .ToList()
                    .Select(g => new
                    {
                        NgayGioDat = g.Select(x => x.NgayGioDat).FirstOrDefault(),
                        ThanhTien = g.Sum(x => decimal.Parse(x.ThanhTien.Replace(" VND", "").Replace(".", "")))
                    })
                    .ToList<dynamic>();
            }

            return Json(revenueData, JsonRequestBehavior.AllowGet);
        }




        //public ActionResult GetRevenueData(string period)
        //{
        //    List<ThongTinDatXe> revenueData = new List<ThongTinDatXe>();

        //    if (period == "day")
        //    {
        //        revenueData = db.ThongTinDatXes
        //            .GroupBy(x => x.NgayGioDat.Date)
        //            .Select(g => new ThongTinDatXe
        //            {
        //                NgayGioDat = g.Key,
        //                ThanhTien = g.Sum(x => x.ThanhTien),
        //                Period = "day"
        //            })
        //            .ToList();
        //    }
        //    else if (period == "week")
        //    {
        //        revenueData = db.ThongTinDatXes
        //            .GroupBy(x => new { Year = GetIso8601WeekOfYear(x.NgayGioDat), Week = x.NgayGioDat.Year })
        //            .Select(g => new ThongTinDatXe
        //            {
        //                NgayGioDat = g.Select(x => x.NgayGioDat).FirstOrDefault(),
        //                ThanhTien = g.Sum(x => x.ThanhTien),
        //                Period = "week"
        //            })
        //            .ToList();
        //    }
        //    else if (period == "month")
        //    {
        //        revenueData = db.ThongTinDatXes
        //            .GroupBy(x => new { Year = x.NgayGioDat.Year, Month = x.NgayGioDat.Month })
        //            .Select(g => new ThongTinDatXe
        //            {
        //                NgayGioDat = g.Select(x => x.NgayGioDat).FirstOrDefault(),
        //                ThanhTien = g.Sum(x => x.ThanhTien),
        //                Period = "month"
        //            })
        //            .ToList();
        //    }
        //    else if (period == "year")
        //    {
        //        revenueData = db.ThongTinDatXes
        //            .GroupBy(x => x.NgayGioDat.Year)
        //            .Select(g => new ThongTinDatXe
        //            {
        //                NgayGioDat = g.Select(x => x.NgayGioDat).FirstOrDefault(),
        //                ThanhTien = g.Sum(x => x.ThanhTien),
        //                Period = "year"
        //            })
        //            .ToList();
        //    }

        //    return Json(revenueData, JsonRequestBehavior.AllowGet);
        //}

        // GET: ThongKe/GetRevenueData
        public ActionResult GetRevenueData2(string period, string selectedDate, string month, string year)
        {
            List<dynamic> revenueData = new List<dynamic>();

            DateTime? date = null;
            if (!string.IsNullOrEmpty(selectedDate))
            {
                date = DateTime.ParseExact(selectedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            if (period == "day")
            {
                revenueData = db.ThongTinDatXes
                    .Where(x => date == null || DbFunctions.TruncateTime(x.NgayGioDat) == date)
                    .GroupBy(x => DbFunctions.TruncateTime(x.NgayGioDat))
                    .Select(g => new
                    {
                        Date = g.Key,
                        RevenueList = g.Select(x => x.ThanhTien)
                    })
                    .AsEnumerable()
                    .Select(x => new
                    {
                        Label = x.Date, // Store the date as-is without formatting
                        TotalRevenue = x.RevenueList.Sum(ParseTotalRevenue)
                    })
                    .ToList<dynamic>();

                // Perform date formatting after retrieving the data
                revenueData = revenueData.Select(x => new
                {
                    Label = x.Label.ToString("dd/MM/yyyy"), // Format the date here
                    TotalRevenue = x.TotalRevenue
                }).ToList<dynamic>();
            }


            else if (period == "week")
            {
                int parsedYear = int.Parse(year);
                int parsedMonth = int.Parse(month);

                var data = db.ThongTinDatXes
                    .Where(x => date == null || (x.NgayGioDat.Year == parsedYear && x.NgayGioDat.Month == parsedMonth))
                    .AsEnumerable()
                    .GroupBy(x => new { Year = x.NgayGioDat.Year, Month = x.NgayGioDat.Month, Week = GetWeekOfMonth(x.NgayGioDat) })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Week = g.Key.Week,
                        RevenueList = g.Select(x => x.ThanhTien)
                    })
                    .ToList();

                revenueData = data
                    .Select(x => new
                    {
                        Label = $"Week {x.Week} ({x.Month}/{x.Year})",
                        TotalRevenue = x.RevenueList.Sum(ParseTotalRevenue)
                    })
                    .ToList<dynamic>();
            }

            else if (period == "month")
            {
                int parsedYear = int.Parse(year);
                int parsedMonth = int.Parse(month);

                revenueData = db.ThongTinDatXes
                    .Where(x => date == null || (x.NgayGioDat.Year == parsedYear && x.NgayGioDat.Month == parsedMonth))
                    .GroupBy(x => new { Year = x.NgayGioDat.Year, Month = x.NgayGioDat.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        RevenueList = g.Select(x => x.ThanhTien)
                    })
                    .AsEnumerable()
                    .Select(x => new
                    {
                        Label = $"{x.Month}/{x.Year}",
                        TotalRevenue = x.RevenueList.Sum(ParseTotalRevenue)
                    })
                    .ToList<dynamic>();
            }
            else if (period == "year")
            {
                int parsedYear = int.Parse(year);

                revenueData = db.ThongTinDatXes
                    .Where(x => date == null || x.NgayGioDat.Year == parsedYear)
                    .GroupBy(x => x.NgayGioDat.Year)
                    .Select(g => new
                    {
                        Year = g.Key,
                        RevenueList = g.Select(x => x.ThanhTien)
                    })
                    .AsEnumerable()
                    .Select(x => new
                    {
                        Label = x.Year.ToString(),
                        TotalRevenue = x.RevenueList.Sum(ParseTotalRevenue)
                    })
                    .ToList<dynamic>();
            }

            return Json(revenueData, JsonRequestBehavior.AllowGet);
        }

        private decimal ParseTotalRevenue(string totalRevenue)
        {
            totalRevenue = totalRevenue.Replace(" VND", "").Replace(".", "");
            return decimal.Parse(totalRevenue, CultureInfo.InvariantCulture);
        }

        private int GetWeekOfMonth(DateTime date)
        {
            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            int daysOffset = (int)firstDayOfMonth.DayOfWeek - 1;
            DateTime firstDayOfWeek = firstDayOfMonth.AddDays(-daysOffset);

            int weekNumber = (date.Day + firstDayOfWeek.Day - 1) / 7 + 1;
            return weekNumber;
        }



    }
    public class RevenueData
    {
        public string NgayGioDat { get; set; }
        public int ThanhTien { get; set; }
    }
}