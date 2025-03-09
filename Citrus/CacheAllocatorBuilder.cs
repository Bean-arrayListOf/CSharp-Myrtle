using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

using RocksDbSharp;

namespace CSharp_Myrtle.Citrus
{
    class CacheAllocatorBuilder : CacheAllocator
    {
        private readonly string root;
        private bool _closed = true;
        private bool autoDelete = true;
        private RocksDb map;
        private string mapFile = "MAINIFEST.CA.MAP";
        public CacheAllocatorBuilder(string baseFile)
        {
            this.root = Path.Combine(baseFile);
            CreateDirectory(this.root);
			this.map = ReadMap();
		}

		public CacheAllocatorBuilder(string baseFile,bool autoDelete)
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

        public CacheFile CacheFileCreate(string fileName,string suffixName)
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
            return new CacheFile(file, uuid);
        }

        public CacheFile? Get(string uuid)
        {
            var value = map.Get(uuid);
            return new CacheFile(Path.Combine(root,value),uuid);
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

        public CacheFile CreateFile(string inputFile)
        {
            var suffixName = Path.GetExtension(inputFile);
            return CacheFileCreate(Kit.GetUUIDv3().ToUpper(),suffixName);
        }

        public void Dispose()
        {
            if (_closed)
            {
				this.map.Dispose();
				if (autoDelete)
                {
					Directory.Delete(this.root);
				}
                _closed = false;
			}
        }
    }
}
