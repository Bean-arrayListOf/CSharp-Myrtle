using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Myrtle.Citrus
{
    [Serializable]
    record CacheFile
    {
        private readonly string file;
        private readonly string uuid;

        public CacheFile(string file,string uuid)
        {
            this.file = file;
            this.uuid = uuid;
        }
    }
}
