using System.IO;
using System.Security.Cryptography;


public static class Encryption 
{
    static Encryption()
    {
        passwordBytes = new byte[]{ 042, 088, 003, 005, 005, 008, 002, 002 };
        saltBytes     = new byte[]{ 001, 006, 004, 006, 124, 111, 007, 034 };
    }
    
    
    // Set your salt here, change it to meet your flavor:
    // The salt bytes must be at least 8 bytes.
    private static readonly byte[] passwordBytes, saltBytes;
    
    
    public static byte[] Encrypt(byte[] bytesToBeEncrypted)
    {
        byte[] encryptedBytes;

        using (MemoryStream ms = new MemoryStream())
        {
            using (RijndaelManaged AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    cs.Close();
                }
                encryptedBytes = ms.ToArray();
            }
        }

        return encryptedBytes;
    }

    public static byte[] Decrypt(byte[] bytesToBeDecrypted)
    {
        byte[] decryptedBytes;

        using (MemoryStream ms = new MemoryStream())
        {
            using (RijndaelManaged AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                    cs.Close();
                }
                decryptedBytes = ms.ToArray();
            }
        }

        return decryptedBytes;
    }
}
