using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Myrtle.Citrus
{
    class MapDB : IDisposable
    {
        private bool _Disposed = true;
        private readonly SQLiteConnection _Connect;
        private readonly string _IndexTableName;
        private readonly string _IndexTableDefaultBDB;
        private readonly ResourceManager _Resource = Assembly.GetExecutingAssembly().GetResource("CSharp_Myrtle.Citrus.MapDBResource");

        public MapDB(string baseFile)
        {
            if (!File.Exists(baseFile))
            {
                SQLiteConnection.CreateFile(baseFile);
            }

            this._Connect = new SQLiteConnection($"Data Source={baseFile};");
            this._Connect.Open();

            this._IndexTableName = _Resource.GetString("_IndexTableName")!;
            this._IndexTableDefaultBDB = _Resource.GetString("_IndexTableDefaultBDB")!;

            using var command = new SQLiteCommand("PRAGMA journal_mode=WAL;", _Connect);
            command.ExecuteNonQuery();

            NeoIndexTable();
        }

        public int NeoIndexTable()
        {
            var sql = String.Format(_Resource.GetString("NeoIndexTable_sql")!, _IndexTableName);
            using var command = new SQLiteCommand(sql, _Connect);
            return command.ExecuteNonQuery();
        }

        public int InsertIndexTable(string db,string key,MapDBType type,string data_table)
        {
            if (db.Length > 500)
            {
                throw new IndexOutOfRangeException("db 长度不得超过500");
            }

            if (key.Length > 500)
            {
                throw new IndexOutOfRangeException("key 长度不得超过500");
            }

            var sql = String.Format(_Resource.GetString("InsertIndexTable_sql")!,_IndexTableName);
            using var command = new SQLiteCommand(sql, _Connect);
            command.Parameters.AddWithValue("@A_DATABASE",db);
            command.Parameters.AddWithValue("@A_KEY", key);
            command.Parameters.AddWithValue("@A_DATA_TABLE", data_table);
            command.Parameters.AddWithValue("@A_DATA_TYPE", ((int)type));
            return command.ExecuteNonQuery();
        }

        public int InsertDataType(string data_table, List<byte[]> datas)
        {
            var sql = String.Format(_Resource.GetString("InsertDataType_sql")!,data_table);
            using var command = new SQLiteCommand(sql,_Connect);
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
            var sql = String.Format(_Resource.GetString("InsertDataType_sql")!, data_table);
            using var command = new SQLiteCommand(sql, _Connect);
            command.Parameters.AddWithValue("@B_DATA", data);
            return command.ExecuteNonQuery();
        }

        public bool ContainsIndexTable(string db,string key)
        {
            var sql = String.Format(_Resource.GetString("ContainsIndexTable_sql")!, _IndexTableName);
            using var command = new SQLiteCommand(sql, _Connect);
            command.Parameters.AddWithValue("@A_DATABASE", db);
            command.Parameters.AddWithValue("@A_KEY", key);
            return command.ExecuteScalar().ToInt() != 0;
            
        }

        public string NeoDataTable()
        {
            var baseTable = GetUUIDv3();
            var sql = String.Format(_Resource.GetString("NeoDataTable_sql")!,baseTable);
            using var command = new SQLiteCommand(sql, _Connect);
            command.ExecuteNonQuery();
            return baseTable;
        }

        public int RemoveDataTable(string data_type)
        {
            var sql = String.Format(_Resource.GetString("RemoveDataTable_sql")!,data_type);
            using var command = new SQLiteCommand(sql, _Connect);
            return command.ExecuteNonQuery();
        }

        public int RemoveIndex(string db,string key)
        {
            var sql = String.Format(_Resource.GetString("RemoveIndex_sql")!, _IndexTableName);
            using var command = new SQLiteCommand(sql,_Connect);
            command.Parameters.AddWithValue("@A_DATABASE", db);
            command.Parameters.AddWithValue("@A_KEY", key);
            return command.ExecuteNonQuery();
        }

        public int UpdateIndexDataTable(string db,string key,string data_table)
        {
            var sql = String.Format(_Resource.GetString("UpdateIndexDataTable_sql")!,_IndexTableName);
            using var command = new SQLiteCommand(sql, _Connect);
            command.Parameters.AddWithValue("@A_DATABASE", db);
            command.Parameters.AddWithValue("@A_KEY", key);
            command.Parameters.AddWithValue("@A_DATA_TABLE", data_table);
            return command.ExecuteNonQuery();
        }

        public string GetDataTable(string db,string key)
        {
            var sql = String.Format(_Resource.GetString("GetDataTable_sql")!, _IndexTableName);
            using var command = new SQLiteCommand(sql, _Connect);
            command.Parameters.AddWithValue("@A_DATABASE", db);
            command.Parameters.AddWithValue("@A_KEY", key);
            using var read = command.ExecuteReader();
            read.Read();
            return read.GetString(0);
        }

        public void Update(string db,string key, byte[] data)
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

        public void Push(string db,string key,MapDBType type, byte[] value)
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

        public byte[]? Get(string db,string key)
        {
            if (!ContainsIndexTable(db, key))
            {
                return null;
            }

            var dataTable = GetDataTable(db, key);

            var sql = String.Format(_Resource.GetString("Get_sql")!,dataTable);
            using var command = new SQLiteCommand(sql, _Connect);
            using var read = command.ExecuteReader();
            read.Read();
            return read.GetStream(1).ReadAllByte();
        }

        public bool Remove(string db,string key)
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

            var sql = String.Format(_Resource.GetString("Get_sql")!, dataTable);
            using var command = new SQLiteCommand(sql, _Connect);
            using var read = command.ExecuteReader();
            var list = new List<byte[]>();
            while (read.Read())
            {
                list.Add(read.GetStream(1).ReadAllByte());
            }
            return list;
        }

        public MapDBType GetType(string db,string key)
        {
            var sql = String.Format(_Resource.GetString("GetType_sql")!, _IndexTableName);
            using var command = new SQLiteCommand(sql, _Connect);
            command.Parameters.AddWithValue("@A_DATABASE", db);
            command.Parameters.AddWithValue("@A_KEY", key); 
            return (MapDBType)command.ExecuteScalar().ToInt();
        }

        public static string GetUUIDv3()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }

        public void Dispose()
        {
            if (this._Disposed)
            {
                this._Connect.Close();
                this._Connect.Dispose();
                _Disposed = false;
            }
        }
    }
}
