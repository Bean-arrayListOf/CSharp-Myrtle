namespace CSharp_Myrtle.Citrus;

public class TestStream : IDisposable
{
    public string print1()
    {
        return "1";
    }

    private void Closed(object? any,EventArgs args)
    {
        Dispose();
    }
    public void Dispose()
    {
        Console.WriteLine("closed");
    }
}