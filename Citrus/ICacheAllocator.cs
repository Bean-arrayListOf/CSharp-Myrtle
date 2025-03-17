namespace CSharp_Myrtle.Citrus
{
    interface ICacheAllocator : IDisposable
    {
        public string GetRoot();
        public (Stream stream, string uuid) CreateStream();
        public (string file, string uuid) CopyFile(string inputFile);
        public (string file, string uuid)? Get(string uuid);
        public List<string> GetKeys();
        public (string file, string uuid) CreateFile(string inputFile);
    }
}