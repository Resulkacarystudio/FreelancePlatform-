using FirebaseAdmin;
using FreelancePlatform.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FreelancePlatform.Helpers
{
    public class FirebaseHelper
    {
        private static bool isInitialized = false;
        private static FirestoreDb? firestoreDb; // ? işareti ile nullable yaptık

        public static void InitializeFirebase()
        {
            if (isInitialized)
                return;

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebase-key.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(path),
                });
            }

            // Burada databaseId parametresi yok
            firestoreDb = FirestoreDb.Create("freelanceplatform-1155a");
            isInitialized = true;
        }

        public static async Task AddDataAsync(string koleksiyon, string belgeId, object veri)
        {
            InitializeFirebase();
            DocumentReference docRef = firestoreDb!.Collection(koleksiyon).Document(belgeId);
            await docRef.SetAsync(veri);
        }

        public static async Task<Dictionary<string, object>?> GetDataAsync(string koleksiyon, string belgeId)
        {
            InitializeFirebase();
            DocumentReference docRef = firestoreDb!.Collection(koleksiyon).Document(belgeId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ToDictionary() : null;
        }

        public static async Task<AppUser?> GetUserByEmailAsync(string email)
        {
            InitializeFirebase();
            DocumentReference docRef = firestoreDb!.Collection("kullanicilar").Document(email);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return null;

            var dict = snapshot.ToDictionary();
            return new AppUser
            {
                KullaniciID = Convert.ToInt32(dict["KullaniciID"]),
                AdSoyad = dict["AdSoyad"].ToString(),
                EmailAdres = dict["EmailAdres"].ToString(),
                Sifre = dict["Sifre"].ToString(),
                Rol = dict["Rol"].ToString()
            };
        }

    }


}
