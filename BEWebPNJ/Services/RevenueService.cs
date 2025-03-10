using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BEWebPNJ.Models;

namespace BEWebPNJ.Services
{
    public class RevenueService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "Orders"; // Tên collection trên Firestore

        public RevenueService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        // 🔹 Tính doanh thu trong tuần
        public async Task<double> GetWeeklyRevenue()
        {
            try
            {
                DateTime startOfWeek = DateTime.UtcNow.AddHours(7).StartOfWeek(DayOfWeek.Monday);
                DateTime endOfWeek = startOfWeek.AddDays(7);

                QuerySnapshot snapshot = await _firestoreDb.Collection(CollectionName)
                    .WhereGreaterThanOrEqualTo("timeOrder", startOfWeek)
                    .WhereLessThan("timeOrder", endOfWeek)
                    .GetSnapshotAsync();

                return snapshot.Documents
                    .Where(doc => doc.Exists)
                    .Select(doc => doc.ConvertTo<Order>())
                    .Where(order => order.status != "process")
                    .Sum(order => order.totalAmount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tính doanh thu tuần: {ex.Message}");
                return 0;
            }
        }

        // 🔹 Tính doanh thu trong tháng
        public async Task<double> GetMonthlyRevenue()
        {
            try
            {
                DateTime startOfMonth = new DateTime(DateTime.UtcNow.AddHours(7).Year, DateTime.UtcNow.AddHours(7).Month, 1);
                DateTime endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1); // Lấy ngày cuối tháng chính xác (23:59:59)

                // Chuyển đổi DateTime thành Firestore Timestamp
                var startOfMonthTimestamp = Timestamp.FromDateTime(startOfMonth.ToUniversalTime());
                var endOfMonthTimestamp = Timestamp.FromDateTime(endOfMonth.ToUniversalTime());

                Console.WriteLine($"Start of Month: {startOfMonth}");
                Console.WriteLine($"End of Month: {endOfMonth}");

                QuerySnapshot snapshot = await _firestoreDb.Collection(CollectionName)
                    .WhereGreaterThanOrEqualTo("timeOrder", startOfMonthTimestamp)
                    .WhereLessThan("timeOrder", endOfMonthTimestamp)
                    .GetSnapshotAsync();

                return snapshot.Documents
                   .Where(doc => doc.Exists)
                   .Select(doc => doc.ConvertTo<Order>())
                   .Where(order => order.status != "process")
                   .Sum(order => order.totalAmount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tính doanh thu tháng: {ex.Message}");
                return 0;
            }
        }

        // 🔹 Tính doanh thu trong quý
        public async Task<double> GetQuarterlyRevenue()
        {
            try
            {
                // Xác định thời gian bắt đầu và kết thúc của quý
                DateTime startOfQuarter = GetStartOfQuarter(DateTime.UtcNow.AddHours(7));
                DateTime endOfQuarter = startOfQuarter.AddMonths(3).AddSeconds(-1); // Lấy thời gian cuối của quý

                // Chuyển đổi DateTime thành Firestore Timestamp
                var startOfQuarterTimestamp = Timestamp.FromDateTime(startOfQuarter.ToUniversalTime());
                var endOfQuarterTimestamp = Timestamp.FromDateTime(endOfQuarter.ToUniversalTime());

                Console.WriteLine($"Start of Quarter: {startOfQuarter}");
                Console.WriteLine($"End of Quarter: {endOfQuarter}");

                // Truy vấn Firestore
                QuerySnapshot snapshot = await _firestoreDb.Collection(CollectionName)
                    .WhereGreaterThanOrEqualTo("timeOrder", startOfQuarterTimestamp)
                    .WhereLessThan("timeOrder", endOfQuarterTimestamp)
                    .GetSnapshotAsync();

                // Tính tổng doanh thu từ các đơn hàng trong quý
                return snapshot.Documents
                   .Where(doc => doc.Exists)
                   .Select(doc => doc.ConvertTo<Order>())
                   .Where(order => order.status != "process")
                   .Sum(order => order.totalAmount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tính doanh thu quý: {ex.Message}");
                return 0;
            }
        }

        // 🔹 Tính doanh thu trong năm
        public async Task<double> GetYearlyRevenue()
        {
            try
            {
                DateTime startOfYear = new DateTime(DateTime.UtcNow.AddHours(7).Year, 1, 1);
                DateTime endOfYear = new DateTime(DateTime.UtcNow.AddHours(7).Year, 12, 31, 23, 59, 59); // Đến ngày cuối năm

                // Chuyển đổi DateTime thành Firestore Timestamp
                var startOfYearTimestamp = Timestamp.FromDateTime(startOfYear.ToUniversalTime());
                var endOfYearTimestamp = Timestamp.FromDateTime(endOfYear.ToUniversalTime());

                Console.WriteLine($"Start of Year: {startOfYear}");
                Console.WriteLine($"End of Year: {endOfYear}");

                // Truy vấn Firestore
                QuerySnapshot snapshot = await _firestoreDb.Collection(CollectionName)
                    .WhereGreaterThanOrEqualTo("timeOrder", startOfYearTimestamp)
                    .WhereLessThan("timeOrder", endOfYearTimestamp)
                    .GetSnapshotAsync();

                // Tính tổng doanh thu từ các đơn hàng trong năm
                return snapshot.Documents
                   .Where(doc => doc.Exists)
                   .Select(doc => doc.ConvertTo<Order>())
                   .Where(order => order.status != "process")
                   .Sum(order => order.totalAmount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tính doanh thu năm: {ex.Message}");
                return 0;
            }
        }

        // 🔹 Hỗ trợ lấy thời gian bắt đầu của quý
        private DateTime GetStartOfQuarter(DateTime date)
        {
            int quarter = (date.Month - 1) / 3 + 1;
            int startMonth = (quarter - 1) * 3 + 1;
            return new DateTime(date.Year, startMonth, 1);
        }
    }

    // Extension method để tính thời gian bắt đầu của tuần
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek)
        {
            int diff = dateTime.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dateTime.AddDays(-diff).Date;
        }
    }
}
