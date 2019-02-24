using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BasicSecurity.Models.Encryption
{
    public static class HybridEncryption
    {
        public static byte[] EncryptMessageWithAes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;

        }

        public static string DecryptMessageWithAes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }

        public static EncryptedAes EncryptAesKeyWithPublicKeyReceiver(Aes aesKey, RSACryptoServiceProvider rsa)
        {

              // Import existing public RSA key.
                byte[] encryptedAesIV = rsa.Encrypt(aesKey.IV, true);
                byte[] encryptedAesKey = rsa.Encrypt(aesKey.Key, true);
                EncryptedAes encryptedAes = new EncryptedAes(encryptedAesIV, encryptedAesKey);                    // Create Aes placeholder
                return encryptedAes;
    
        }

        public static Aes DecryptAesKeyWithPrivateKeyReceiver(EncryptedAes encryptedAes, RSACryptoServiceProvider rsa)
        {

                byte[] decryptedAesIV = rsa.Decrypt(encryptedAes.IV, true);
                byte[] decryptedAesKey = rsa.Decrypt(encryptedAes.Key, true);
                Aes decryptedAes = Aes.Create();                    // Create Aes placeholder
                decryptedAes.IV = decryptedAesIV;                   // Put IV and Key in placeholder
                decryptedAes.Key = decryptedAesKey;
                return decryptedAes;
        }

        public static string GenerateHash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

        public static byte[] HashAndSignMessage(string message, RSACryptoServiceProvider rsa)
        {
                // Import existing public RSA key.   
                return rsa.SignData(Encoding.UTF8.GetBytes(message), new SHA1CryptoServiceProvider());

        }

        public static bool VerifySignedHash(string message, byte[] enctyptedHash, RSACryptoServiceProvider rsa)
        {

                return rsa.VerifyData(Encoding.UTF8.GetBytes(message), new SHA1CryptoServiceProvider(), enctyptedHash);
        }
    }


}