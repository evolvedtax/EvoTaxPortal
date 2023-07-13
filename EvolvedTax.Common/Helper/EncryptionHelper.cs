using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EvolvedTax.Helpers
{
    public static class KeyGenerator
    {
        public static byte[] GenerateKey(int keySize)
        {
            using (var generator = RandomNumberGenerator.Create())
            {
                byte[] key = new byte[keySize / 8];
                generator.GetBytes(key);
                return key;
            }
        }
    }
    public static class EncryptionHelper
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("JBSWY3DPEHPK3PXP");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("JBSWY3DPEHPK3PXP");
        public static string Encrypt(string plainText)
        {
            byte[] encryptedBytes;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptoStream))
                        {
                            writer.Write(plainText);
                        }

                        encryptedBytes = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            string plainText = string.Empty;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream))
                        {
                            plainText = reader.ReadToEnd();
                        }
                    }
                }
            }
            return plainText;
        }
    }
}
