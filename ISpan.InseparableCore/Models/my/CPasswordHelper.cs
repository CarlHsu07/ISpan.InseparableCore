using System.Security.Cryptography;
using System.Text;

namespace ISpan.InseparableCore.Models.my
{
    public class CPasswordHelper
    {
        // 生成隨機的鹽值
        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16]; // 推薦使用 16 個位元組 (128 位元) 的鹽值
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        // 將密碼與鹽值結合後進行加密
        public static byte[] HashPasswordWithSalt(byte[] password, byte[] salt)
        {
            using (var sha256 = new SHA256Managed())
            {
                byte[] saltedPassword = new byte[password.Length + salt.Length];
                Buffer.BlockCopy(password, 0, saltedPassword, 0, password.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, password.Length, salt.Length);
                return sha256.ComputeHash(saltedPassword);
            }
        }

        // 驗證密碼是否相符
        static bool VerifyPassword(string inputPassword, string hashedPasswordString, string saltString)
        {
            // 將 Base64 字串形式的鹽值轉換為 byte 陣列
            byte[] salt = Convert.FromBase64String(saltString);

            // 計算輸入密碼和鹽值結合後的雜湊值
            byte[] computedHash = HashPasswordWithSalt(Encoding.UTF8.GetBytes(inputPassword), salt);
            string computedHashString = Convert.ToBase64String(computedHash);

            // 比較計算得到的雜湊值與儲存的雜湊值是否相符
            return computedHashString == hashedPasswordString;
        }
    }
}
