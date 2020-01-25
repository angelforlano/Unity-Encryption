using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

public static class Secure
{
    const int IvLength = 16;
    const int keySize = 32;
    const string keyString = "gDpTU3CAW2m6WT8Q"; // gDpTU3CAW2m6WT8Q
    
    static readonly UTF8Encoding encoder;
    static readonly AesManaged aes;

    static Secure()
    {
        encoder = new UTF8Encoding();
        aes = new AesManaged{ Key = encoder.GetBytes(keyString).Take(keySize).ToArray() };
        aes.BlockSize = IvLength * 8;
    }

    public static byte[] ShowKey()
    {
        return encoder.GetBytes(keyString).Take(keySize).ToArray();
    }

    public static byte[] GenerateIV()
    {
        aes.GenerateIV();
        return aes.IV;
    }

    public static byte[] Encrypt(byte[] buffer)
    {
        aes.GenerateIV();
        using (ICryptoTransform encryptor = aes.CreateEncryptor())
        {
            byte[] inputBuffer = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return aes.IV.Concat(inputBuffer).ToArray();
        }
    }

    public static byte[] Decrypt(byte[] buffer)
    {
        byte[] iv = buffer.Take(IvLength).ToArray();
        using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv))
        {
            return decryptor.TransformFinalBlock(buffer, IvLength, buffer.Length - IvLength);
        }
    }

    public static string Encrypt(string unencrypted)
    {
        return Convert.ToBase64String(Encrypt(encoder.GetBytes(unencrypted)));
    }

    public static string EncryptByUserPassword(string unencrypted, string password)
    {
        return Convert.ToBase64String(EncryptStringToBytes_Aes(unencrypted, encoder.GetBytes(password), GenerateIV()));
        //return Convert.ToBase64String(Encrypt(encoder.GetBytes(unencrypted)));
    }

    public static string DecryptString(byte[] encrypted)
    {
        byte[] bytesDecrypted = Decrypt(encrypted);
        return encoder.GetString(bytesDecrypted, 0, bytesDecrypted.Length);
    }

    public static string DecryptString(string encrypted)
    {
        return DecryptString(Convert.FromBase64String(encrypted));
    }

    public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        byte[] encrypted;
        
        // Create an AesManaged object
        // with the specified key and IV.
        using (AesManaged aesAlg = new AesManaged())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return encrypted;
    }
}