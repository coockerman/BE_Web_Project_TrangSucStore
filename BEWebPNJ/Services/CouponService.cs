using BEWebPNJ.Models;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BEWebPNJ.Services
{
    public class CouponService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "Coupons"; // Assuming the collection name for coupons

        public CouponService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        // ✅ Lấy danh sách tất cả Coupons
        public async Task<List<Coupon>> GetAllCouponsAsync()
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Coupon>()).ToList();
        }

        // ✅ Lấy thông tin Coupon theo ID
        public async Task<Coupon?> GetCouponByIdAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<Coupon>() : null;
        }

        // ✅ Thêm hoặc cập nhật Coupon
        public async Task<bool> SetCouponAsync(Coupon coupon)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(coupon.id);
            await docRef.SetAsync(coupon, SetOptions.MergeAll);
            return true;
        }

        // ✅ Cập nhật Coupon (chỉ cập nhật trường cần thiết)
        public async Task<bool> UpdateCouponAsync(string id, Dictionary<string, object> updates)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            await docRef.UpdateAsync(updates);
            return true;
        }

        // ✅ Xóa Coupon theo ID
        public async Task<bool> DeleteCouponAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            await docRef.DeleteAsync();
            return true;
        }
    }
}
