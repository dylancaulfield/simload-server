using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SimLoad.Common.Options;

namespace SimLoad.Server.Common.Encryption;

public interface IEncryptor
{
    string Encrypt(object value);
    T Decrypt<T>(string value);
}

public class Encryptor : IEncryptor
{
    private readonly EncryptionOptions _encryptionOptions;

    public Encryptor(IOptions<EncryptionOptions> encryptionOptions)
    {
        _encryptionOptions = encryptionOptions.Value;
    }

    public string Encrypt(object value)
    {
        var payload = JsonConvert.SerializeObject(value);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_encryptionOptions.SymmetricKey);
        aes.IV = GenerateIV();

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(payload);
        }

        var array = ms.ToArray();
        var combined = array.Combine(aes.IV);
        return Convert.ToBase64String(combined);
    }

    public T Decrypt<T>(string encrypted)
    {
        var buffer = Convert.FromBase64String(encrypted);
        var (iv, payload) = buffer.Split();

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_encryptionOptions.SymmetricKey);
        aes.IV = iv;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        string output;
        using (var ms = new MemoryStream(payload))
        {
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            {
                using (var sr = new StreamReader(cs))
                {
                    output = sr.ReadToEnd();
                }
            }
        }

        return JsonConvert.DeserializeObject<T>(output);
    }

    /// <summary>
    ///     Generate a random initialization vector
    /// </summary>
    /// <returns></returns>
    private static byte[] GenerateIV()
    {
        var iv = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(iv);
        return iv;
    }
}

/// <summary>
///     Extension methods to split and combine byte array into IV and payload
/// </summary>
internal static class ByteArrayExtension
{
    private const int PrefixLength = 16;

    internal static (byte[], byte[]) Split(this byte[] bytes)
    {
        var iv = new byte[PrefixLength];
        var payload = new byte[bytes.Length - PrefixLength];

        Array.Copy(bytes, iv, PrefixLength);
        Array.Copy(bytes, PrefixLength, payload, 0, bytes.Length - PrefixLength);

        return (iv, payload);
    }

    internal static byte[] Combine(this byte[] bytes, byte[] iv)
    {
        var combined = new byte[bytes.Length + iv.Length];
        Array.Copy(iv, combined, iv.Length);
        Array.Copy(bytes, 0, combined, iv.Length, bytes.Length);
        return combined;
    }
}