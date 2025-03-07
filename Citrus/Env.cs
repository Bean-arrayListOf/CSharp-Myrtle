namespace CSharp_Myrtle.Citrus;

public static class Env
{
    public static string? Get(string key)
    {
        return Environment.GetEnvironmentVariable(key);
    }
}