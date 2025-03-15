using System.Security.Cryptography;

namespace CSharp_Myrtle.Citrus;

public class Hash
{
    public static byte[] Get(HashMode? mode, Stream data)
    {
        using var sha = GetSHA(mode);
        return sha.ComputeHash(data);
    }

    public static byte[] Get(HashMode? mode, byte[] data)
    {
        using var sha = GetSHA(mode);
        return sha.ComputeHash(data);
    }

    public static byte[] Get(HashMode? mode, string data) => Get(mode, data.EncBytes());

    public static string GetToHex(HashMode? mode, byte[] data) => ToHex(Get(mode, data));

    public static string GetToHex(HashMode? mode, Stream data) => ToHex(Get(mode, data));

    public static string GetToHex(HashMode? mode, string data) => GetToHex(mode, data.EncBytes());

    public static string GetToBase64(HashMode? mode, byte[] data) => ToBase64(Get(mode, data));

    public static string GetToBase64(HashMode? mode, Stream data) => ToBase64(Get(mode, data));

    public static string GetToBase64(HashMode? mode, string data) => ToBase64(Get(mode, data));

    public static HashAlgorithm GetSHA(HashMode? mode)
    {
        if (mode == null)
        {
            return GetDefaultSHA();
        }

        return mode switch
        {
            HashMode.SHA1 => SHA1.Create(),
            HashMode.SHA256 => SHA256.Create(),
            HashMode.SHA384 => SHA384.Create(),
            HashMode.SHA512 => SHA512.Create(),
            HashMode.SHA3_256 => SHA3_256.Create(),
            HashMode.SHA3_384 => SHA3_384.Create(),
            HashMode.SHA3_512 => SHA3_512.Create(),
            _ => GetDefaultSHA()
        };
    }

    public static HashAlgorithm GetDefaultSHA()
    {
        return (HashMode)Env.cr.GetString("sha")!.ToInt() switch
        {
            HashMode.SHA1 => SHA1.Create(),
            HashMode.SHA256 => SHA256.Create(),
            HashMode.SHA384 => SHA384.Create(),
            HashMode.SHA512 => SHA512.Create(),
            HashMode.SHA3_256 => SHA3_256.Create(),
            HashMode.SHA3_384 => SHA3_384.Create(),
            HashMode.SHA3_512 => SHA3_512.Create(),
            _ => SHA256.Create()
        };
    }

    static string ToHex(byte[] data) => data.HexString();

    static string ToBase64(byte[] data) => Convert.ToBase64String(data);
}