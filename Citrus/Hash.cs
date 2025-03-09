using System.Security.Cryptography;

namespace CSharp_Myrtle.Citrus;

public class Hash
{
    public static byte[] Get(HashMode mode, Stream data)
    {
        HashAlgorithm sha;
        switch (mode)
        {
            case HashMode.SHA1:
            {
                sha = SHA1.Create();
                break;
            }
            case HashMode.SHA3_256:
            {
                sha = SHA3_256.Create();
                break;
            }
            case HashMode.SHA3_384:
            {
                sha = SHA3_384.Create();
                break;
            }
            case HashMode.SHA3_512:
            {
                sha = SHA3_512.Create();
                break;
            }
            case HashMode.SHA256:
            {
                sha = SHA256.Create();
                break;
            }
            case HashMode.SHA384:
            {
                sha = SHA384.Create();
                break;
            }
            case HashMode.SHA512:
            {
                sha = SHA512.Create();
                break;
            }
            default:
            {
                sha = SHA256.Create();
                break;
            }
        }

        var cache = sha.ComputeHash(data);
        sha.Dispose();
        return cache;
    }

    public static byte[] Get(HashMode mode, byte[] data)
    {
        using var ms = new MemoryStream(data);
        return Get(mode, ms);
    }
}