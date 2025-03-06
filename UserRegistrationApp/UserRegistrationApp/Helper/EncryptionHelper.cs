using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Helper class for encryption and decryption using AES algorithm.
/// </summary>
public static class EncryptionHelper
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("your-32-char-key-here"); // 32 bytes key for AES-256
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("your-16-char-iv-here"); // 16 bytes IV for AES

    /// <summary>
    /// Encrypts the specified plain text using AES algorithm.
    /// </summary>
    /// <param name="plainText">The plain text to encrypt.</param>
    /// <returns>The encrypted text as a base64 string.</returns>
    public static string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    /// <summary>
    /// Decrypts the specified encrypted text using AES algorithm.
    /// </summary>
    /// <param name="cipherText">The encrypted text as a base64 string.</param>
    /// <returns>The decrypted plain text.</returns>
    public static string Decrypt(string cipherText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}

