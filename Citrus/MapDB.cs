using System.Data.SQLite;
using System.Reflection;
using System.Resources;

// dev
namespace CSharp_Myrtle.Citrus
{
    class MapDB : AutoClosed
    {
        //private bool _disposed = true;
        private readonly SQLiteConnection _connect;
        private readonly string _indexTableDefaultBDB;
        private readonly string _indexTableName;

        private readonly ResourceManager _resource =
            Assembly.GetExecutingAssembly().GetResource("CSharp_Myrtle.Citrus.MapDBResource");

        public MapDB(string baseFile)
        {
            if (!File.Exists(baseFile))
            {
                SQLiteConnection.CreateFile(baseFile);
            }

            this._connect = new SQLiteConnection($"Data Source={baseFile};");
            this._connect.Open();

            this._indexTableName = _resource.GetString("_indexTableName")!;
            this._indexTableDefaultBDB = _resource.GetString("_indexTableDefaultBDB")!;

            using var command = new SQLiteCommand("PRAGMA journal_mode=WAL;", _connect);
            command.ExecuteNonQuery();

            NeoIndexTable();
        }

        public int NeoIndexTable()
        {
            var sql = String.Format(_resource.GetString("NeoIndexTable_sql")!, _indexTableName);
            using var command = new SQLiteCommand(sql, _connect);
            return command.ExecuteNonQuery();
        }

        public int InsertIndexTable(string db, string key, MapDBType type, string data_table)
        {
            if (db.Length > 500)
            {
                throw new IndexOutOfRangeException("db 长度不得超过500");
            }

            if (key.Length > 500)
            {
                throw new IndexOutOfRangeException("key 长度不得超过500");
            }

            var sql = String.Format(_resource.GetString("InsertIndexTable_sql")!, _indexTableName);
            using var command = new SQLiteCommand(sql, _connect);
            command.Parameters.AddWithValue("@A_DATABASE", db);
            command.Parameters.AddWithValue("@A_KEY", key);
            command.Parameters.AddWithValue("@A_DATA_TABLE", data_table);
            command.Parameters.AddWithValue("@A_DATA_TYPE", ((int)type));
            return command.ExecuteNonQuery();
        }

        public int InsertDataType(string data_table, List<byte[]> datas)
        {
            var sql = String.Format(_resource.GetString("InsertDataType_sql")!, data_table);
            using var command = new SQLiteCommand(sql, _connect);
            var exitCode = 0;
            foreach (byte[] i in datas)
            {
                command.Parameters.AddWithValue("@B_DATA", i);
                var code = command.ExecuteNonQuery();
                if (code <= 0)
                {
                    exitCode = code;
                    break;
                }
            }

            return exitCode;
        }

        public int InsertDataType(string data_table, byte[] data)
        {
            var sql = String.Format(_resource.GetString("InsertDataType_sql")!, data_table);
            using var command = new SQLiteCommand(sql, _connect);
            command.Parameters.AddWithValue("@B_DATA", data);
            return command.ExecuteNonQuery();
        }

        public bool ContainsIndexTable(string db, string key)
        {
            var sql = String.Format(_resource.GetString("ContainsIndexTable_sql")!, _indexTableName);
            using var command = new SQLiteCommand(sql, _connect);
            command.Parameters.AddWithValue("@A_DATABASE", db);
            command.Parameters.AddWithValue("@A_KEY", key);
            return command.ExecuteScalar().ToInt() != 0;
        }

        public string NeoDataTable()
        {
            var baseTable = GetUUIDv3();
            var sql = String.Format(_resource.GetString("NeoDataTable_sql")!, baseTable);
            using var command = new SQLiteCommand(sql, _connect);
            command.ExecuteNonQuery();
            return baseTable;
        }

        public int RemoveDataTable(string data_type)
        {
            var sql = String.Format(_resource.GetString("RemoveDataTable_sql")!, data_type);
            using var command = new SQLiteCommand(sql, _connect);
            return command.ExecuteNonQuery();
        }

        public int RemoveIndex(string db, string key)
        {
            var sql = String.Format(_resource.GetString("RemoveIndex_sql")!, _indexTableName);
            using var command = new SQLiteCommand(sql, _connect);
            command.Parameters.AddWithValue("@A_DATABASE", db);
            command.Parameters.AddWithValue("@A_KEY", key);
            return command.ExecuteNonQuery();
        }

        public int UpdateIndexDataTable(string db, string key, string data_table)
        {
            var sql = String.Format(_resource.GetString("UpdateIndexDataTable_sql")!, _indexTableName);
            using var command = new SQLiteCommand(sql, _connect);
            command.Parameters.AddWithValue("@A_DATABASE", db);
            command.Parameters.AddWithValue("@A_KEY", key);
            command.Parameters.AddWithValue("@A_DATA_TABLE", data_table);
            return command.ExecuteNonQuery();
        }

        public string GetDataTable(string db, string key)
        {
            var sql = String.Format(_resource.GetString("GetDataTable_sql")!, _indexTableName);
            using var command = new SQLiteCommand(sql, _connect);
            command.Parameters.AddWithValue("@A_DATABASE", db);
            command.Parameters.AddWithValue("@A_KEY", key);
            using var read = command.ExecuteReader();
            read.Read();
            return read.GetString(0);
        }

        public void Update(string db, string key, byte[] data)
        {
            var dataTableName = GetDataTable(db, key);

            RemoveDataTable(dataTableName);

            //RemoveIndex(db, key);

            var newDataTableName = NeoDataTable();

            UpdateIndexDataTable(db, key, newDataTableName);

            InsertDataType(newDataTableName, data);
        }

        public void Insert(string db, string key, MapDBType type, byte[] value)
        {
            var dataTableName = NeoDataTable();

            InsertDataType(dataTableName, value);

            InsertIndexTable(db, key, type, dataTableName);
        }

        public void Push(string db, string key, MapDBType type, byte[] value)
        {
            if (ContainsIndexTable(db, key))
            {
                Update(db, key, value);
            }
            else
            {
                Insert(db, key, type, value);
            }
        }

        public byte[]? Get(string db, string key)
        {
            if (!ContainsIndexTable(db, key))
            {
                return null;
            }

            var dataTable = GetDataTable(db, key);

            var sql = String.Format(_resource.GetString("Get_sql")!, dataTable);
            using var command = new SQLiteCommand(sql, _connect);
            using var read = command.ExecuteReader();
            read.Read();
            return read.GetStream(1).ReadAllByte();
        }

        public bool Remove(string db, string key)
        {
            if (!ContainsIndexTable(db, key))
            {
                return false;
            }

            var dataTable = GetDataTable(db, key);
            RemoveDataTable(dataTable);
            RemoveIndex(db, key);
            return true;
        }

        public List<byte[]> Gets(string db, string key)
        {
            if (!ContainsIndexTable(db, key))
            {
                return [];
            }

            var dataTable = GetDataTable(db, key);

            var sql = String.Format(_resource.GetString("Get_sql")!, dataTable);
            using var command = new SQLiteCommand(sql, _connect);
            using var read = command.ExecuteReader();
            var list = new List<byte[]>();
            while (read.Read())
            {
                list.Add(read.GetStream(1).ReadAllByte());
            }

            return list;
        }

        public MapDBType GetType(string db, string key)
        {
            var sql = String.Format(_resource.GetString("GetType_sql")!, _indexTableName);
            using var command = new SQLiteCommand(sql, _connect);
            command.Parameters.AddWithValue("@A_DATABASE", db);
            command.Parameters.AddWithValue("@A_KEY", key);
            return (MapDBType)command.ExecuteScalar().ToInt();
        }

        public static string GetUUIDv3()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }

        protected override void Close()
        {
            this._connect.Close();
            this._connect.Dispose();
        }
    }
}