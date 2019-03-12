namespace CN88.CustomerSetting.SettingType1.Helpers
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public interface IAseEncryptor
    {
        string Decrypt(string cipherText, string secretKey);

        string Encrypt(string plainText, string secretKey);
    }

    public class AseEncryptor : IAseEncryptor
    {
        /// <summary>
        /// Private secret key and initialization vector constant. This value contains 12 characters.
        /// And will be added more 4 characters in secretKey parameter when calling method Encrypt or Decrypt.
        /// </summary>
        private const string keyVector = "a5s8d2e9c172";

        /// <summary>
        /// Decrypts an encrypted text to plain text.
        /// </summary>
        /// <param name="cipherText">Encrypted text.</param>
        /// <param name="secretKey">A value must be 4 characters.</param>
        /// <returns>Plain text.</returns>
        public string Decrypt(string cipherText, string secretKey)
        {
            var key = Encoding.UTF8.GetBytes(keyVector + secretKey);
            var iv = key;

            var encrypted = Convert.FromBase64String(cipherText.Replace(" ", "+"));
            var decrypted = DecryptStringFromBytes(encrypted, key, iv);

            return decrypted;
        }

        /// <summary>
        /// Encrypts plain text.
        /// </summary>
        /// <param name="plainText">Text to encrypt.</param>
        /// <param name="secretKey">A value must be 4 characters.</param>
        /// <returns>Encrypted text.</returns>
        public string Encrypt(string plainText, string secretKey)
        {
            var key = Encoding.UTF8.GetBytes(keyVector + secretKey);
            var iv = key;
            var decrypted = EncryptStringToBytes(plainText, key, iv);

            return Convert.ToBase64String(decrypted);
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException(nameof(cipherText));
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(iv));
            }

            string plaintext;
            using (var rijAlg = new AesManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                try
                {
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
                catch
                {
                    return string.Empty;
                }
            }

            return plaintext;
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                throw new ArgumentNullException(nameof(plainText));
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(iv));
            }

            byte[] encrypted;
            using (var rijAlg = new AesManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                using (var msEncrypt = new MemoryStream())
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }

            return encrypted;
        }
    }
}