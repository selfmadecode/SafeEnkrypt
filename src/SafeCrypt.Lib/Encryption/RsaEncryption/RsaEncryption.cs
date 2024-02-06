﻿using System.Security.Cryptography;
using System.Text;
using System;
using System.Threading.Tasks;
using SafeCrypt.RsaEncryption.Models;

namespace SafeCrypt.RsaEncryption
{
    public class RsaEncryption
    {
        /// <summary>
        /// Generates RSA key pair.
        /// </summary>
        /// <param name="keySize">The size of the key pair (e.g., 1024, 2048 bits).</param>
        /// <returns>The generated RSA key pair.</returns>
        public static Tuple<string, string> GenerateRsaKeys(int keySize)
        {
            using (var rsa = new RSACryptoServiceProvider(keySize))
            {
                string publicKey = rsa.ToXmlString(false); // Don't include private key
                string privateKey = rsa.ToXmlString(true); // Include private key

                return new Tuple<string, string>(publicKey, privateKey);
            }
        }

        /// <summary>
        /// Encrypts data using RSA public key.
        /// </summary>
        /// <param name="data">The data to be encrypted.</param>
        /// <param name="publicKey">The RSA public key.</param>
        /// <returns>The encrypted data.</returns>
        public static async Task<RsaEncryptionResult> EncryptAsync(string data, string publicKey)
        {
            var result = new RsaEncryptionResult();

            try
            {
                var encryptedData = await Task.Run(() =>
                {
                    using (var rsa = new RSACryptoServiceProvider())
                    {
                        rsa.FromXmlString(publicKey);
                        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                        return rsa.Encrypt(dataBytes, false);
                    }
                });

                result.EncryptedData = encryptedData;
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Decrypts data using RSA private key.
        /// </summary>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <param name="privateKey">The RSA private key.</param>
        /// <returns>The decrypted data.</returns>
        public static async Task<string> DecryptAsync(byte[] encryptedData, string privateKey)
        {
            return await Task.Run(() =>
            {
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(privateKey);
                    byte[] decryptedData = rsa.Decrypt(encryptedData, false);
                    return Encoding.UTF8.GetString(decryptedData);
                }
            });
        }
    }
}