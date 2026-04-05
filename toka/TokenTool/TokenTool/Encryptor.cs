namespace TokenTool
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class Encryptor
    {
        // disable AesManaged is obsolete warnings
#pragma warning disable SYSLIB0021

        public static byte[] GetSha256Hash(string text)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(text);

            byte[] result = GetSha256Hash(keyBytes);

            return result;
        }

        public static byte[] GetSha256Hash(byte[] bytes)
        {
            var sha256 = new SHA256Managed();
            byte[] result = sha256.ComputeHash(bytes);

            return result;
        }

        public static string Encrypt(byte[] keyBytes, string text)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(text);

            byte[] ivEncryptedBytes = Encrypt(keyBytes, plainBytes);

            string ivEncryptedBase64 = Convert.ToBase64String(ivEncryptedBytes);
            return ivEncryptedBase64;
        }

        public static byte[] Encrypt(byte[] key, byte[] plainBytes)
        {
            using (AesManaged aesManaged = CreateAesManagedObject(key))
            {
                ICryptoTransform encryptor = aesManaged.CreateEncryptor();
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                int length = aesManaged.IV.Length + encryptedBytes.Length;

                byte[] result = new byte[length];
                Array.Copy(aesManaged.IV, result, aesManaged.IV.Length);
                Array.Copy(encryptedBytes, 0, result, aesManaged.IV.Length, encryptedBytes.Length);
                return result;
            }
        }

        public static string DecryptToPlain(byte[] key, string ivEncryptedBase64)
        {
            string base64 = DecryptToBase64(key, ivEncryptedBase64);

            byte[] bytes = Convert.FromBase64String(base64);

            string plain = Encoding.UTF8.GetString(bytes);

            return plain;
        }

        private static string DecryptToBase64(byte[] key, string ivEncryptedBase64)
        {
            byte[] encryptedBytes = Convert.FromBase64String(ivEncryptedBase64);

            byte[] plainBytes = Decrypt(key, encryptedBytes);

            string plainBase64 = Convert.ToBase64String(plainBytes);

            return plainBase64;
        }

        private static byte[] Decrypt(byte[] key, byte[] encrypedBytes)
        {
            byte[] iv = new byte[16];
            Array.Copy(encrypedBytes, iv, 16);
            using (AesManaged aesManaged = CreateAesManagedObject(key, iv))
            {
                ICryptoTransform decryptor = aesManaged.CreateDecryptor();
                byte[] unencryptedData = decryptor.TransformFinalBlock(encrypedBytes, 16, encrypedBytes.Length - 16);
                aesManaged.Dispose();
                return unencryptedData;
            }
        }

        private static AesManaged CreateAesManagedObject(byte[] key, byte[] iv = null)
        {
            var aesManaged = new AesManaged();

            aesManaged.Mode = CipherMode.CBC;
            aesManaged.Padding = PaddingMode.PKCS7;
            aesManaged.BlockSize = 128;
            aesManaged.KeySize = 256;
            aesManaged.Key = key;

            if (iv != null)
            {
                aesManaged.IV = iv;
            }

            return aesManaged;
        }
    }
}
