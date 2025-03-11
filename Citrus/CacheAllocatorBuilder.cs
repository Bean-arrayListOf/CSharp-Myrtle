using RocksDbSharp;

namespace CSharp_Myrtle.Citrus
{
	class CacheAllocatorBuilder : CacheAllocator
	{
		private readonly string root;
		private bool _closed = true;
		private bool autoDelete = false;
		private RocksDb map;
		private string mapFile = Env.cr.GetString("Citrus_CacheAllocatorBuilder_mapFileName")!;
		public CacheAllocatorBuilder(string baseFile)
		{
			this.root = Path.Combine(baseFile);
			CreateDirectory(this.root);
			this.map = ReadMap();
		}

		public CacheAllocatorBuilder(string baseFile, bool autoDelete)
		{
			this.root = Path.Combine(baseFile);
			CreateDirectory(this.root);
			this.autoDelete = autoDelete;
			this.map = ReadMap();
		}

		public CacheAllocatorBuilder()
		{
			this.root = Path.Combine(Env.TempPath, Kit.GetUUIDv3().ToUpper());
			CreateDirectory(this.root);
			this.map = ReadMap();
		}

		public CacheAllocatorBuilder(bool autoDelete)
		{
			this.root = Path.Combine(Env.TempPath, Kit.GetUUIDv3().ToUpper());
			CreateDirectory(this.root);
			this.autoDelete = autoDelete;
			this.map = ReadMap();
		}

		public RocksDb ReadMap()
		{
			var mapFile = Path.Combine(this.root, this.mapFile);
			var options = new DbOptions().SetCreateIfMissing(true);
			return RocksDb.Open(options, mapFile);
		}

		public string GetRoot()
		{
			return this.root;
		}

		public static void CreateDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		public (Stream stream, string uuid) CreateStream()
		{
			var stream = CacheFileCreate(Kit.GetUUIDv3(), "stream");
			return (new FileStream(stream.file, FileMode.OpenOrCreate, FileAccess.ReadWrite), stream.uuid);
		}

		public (string file, string uuid) CopyFile(string inputFile)
		{
			var suffixName = Path.GetExtension(inputFile);
			var fileName = Path.GetFileName(inputFile);
			var path = Path.Combine(root, Kit.GetUUIDv3().ToUpper());
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			var file = Path.Combine(path, $"{fileName}.${suffixName}");
			File.Copy(inputFile, file);
			var uuid = Kit.GetUUIDv3().ToUpper();
			map.Put(uuid, file.Replace(root, ""));
			return (file, uuid);
		}

		public (string file, string uuid) CacheFileCreate(string fileName, string suffixName)
		{
			var path = Path.Combine(root, Kit.GetUUIDv3().ToUpper());
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			var file = Path.Combine(path, $"{fileName}.${suffixName}");
			if (!File.Exists(file))
			{
				File.Create(file);
			}
			var uuid = Kit.GetUUIDv3().ToUpper();
			map.Put(uuid, file.Replace(root, ""));
			return (file, uuid);
		}

		public (string file, string uuid)? Get(string uuid)
		{
			var value = map.Get(uuid);
			return (Path.Combine(root, value), uuid);
		}

		public List<string> GetKeys()
		{
			using var map = this.map.NewIterator();
			var keys = new List<string>();
			map.SeekToFirst();
			while (map.Valid())
			{
				keys.Add(map.StringKey());
				map.Next();
			}
			return keys;
		}

		public (string file, string uuid) CreateFile(string inputFile)
		{
			var suffixName = Path.GetExtension(inputFile);
			return CacheFileCreate(Kit.GetUUIDv3().ToUpper(), suffixName);
		}

		public void Dispose()
		{
			if (_closed)
			{
				this.map.Dispose();
				if (autoDelete)
				{
					Directory.Delete(this.root, true);
				}
				_closed = false;
			}
		}

	}
}
