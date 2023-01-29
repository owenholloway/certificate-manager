using System.Security.Cryptography;

namespace CertificateManager.Features.Certificate;

public class Encryption
{
    private static readonly byte[] Salt = new byte[] { 10, 20, 30, 40, 50, 60, 70, 80 };

    public static byte[] Encrypt(byte[] data)
    {
        using var aes = Aes.Create();
        var key = new Rfc2898DeriveBytes(GetEncryptionPassword(), Salt);

        aes.Key = key.GetBytes(aes.KeySize / 8);
        aes.IV = key.GetBytes(aes.BlockSize / 8);

        using var memoryStream = new MemoryStream();
        using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cryptoStream.Write(data, 0, data.Length);
        }

        return memoryStream.ToArray();
    }

    public static byte[] Decrypt(byte[] data)
    {
        using var aes = Aes.Create();
        var key = new Rfc2898DeriveBytes(GetEncryptionPassword(), Salt);

        aes.Key = key.GetBytes(aes.KeySize / 8);
        aes.IV = key.GetBytes(aes.BlockSize / 8);

        using var memoryStream = new MemoryStream();
        using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
        {
            cryptoStream.Write(data, 0, data.Length);
        }

        return memoryStream.ToArray();
    }

    private static string GetEncryptionPassword()
    {
        return Environment.GetEnvironmentVariable("CERT_ENCRYPTION_KEY") ?? "CERT_ENCRYPTION_KEY";
    }
    
}