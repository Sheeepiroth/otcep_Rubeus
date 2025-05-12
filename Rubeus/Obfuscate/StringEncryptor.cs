using System;
using System.Text;

namespace Rubeus.Obfuscate
{
    public static class StringEncryptor
    {
        // XOR encryption key
        private static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("B3@t_5tr1nG_An@ly51s!");

        /// <summary>
        /// Encrypts a string using XOR encryption and encodes the result in Base64
        /// </summary>
        /// <param name="plainText">The string to encrypt</param>
        /// <returns>Base64 encoded encrypted string</returns>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = new byte[plainTextBytes.Length];

            for (int i = 0; i < plainTextBytes.Length; i++)
            {
                encryptedBytes[i] = (byte)(plainTextBytes[i] ^ EncryptionKey[i % EncryptionKey.Length]);
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Decrypts a Base64 encoded, XOR encrypted string
        /// </summary>
        /// <param name="encryptedText">The Base64 encoded encrypted string</param>
        /// <returns>The decrypted string</returns>
        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return encryptedText;

            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] decryptedBytes = new byte[encryptedBytes.Length];

                for (int i = 0; i < encryptedBytes.Length; i++)
                {
                    decryptedBytes[i] = (byte)(encryptedBytes[i] ^ EncryptionKey[i % EncryptionKey.Length]);
                }

                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch
            {
                // If unable to decrypt (e.g., not a valid Base64 string or wrong key), return the original string
                // This might not be the desired behavior in all cases.
                // For stricter error handling, you might want to throw an exception or return null.
                return encryptedText; 
            }
        }
    }
}