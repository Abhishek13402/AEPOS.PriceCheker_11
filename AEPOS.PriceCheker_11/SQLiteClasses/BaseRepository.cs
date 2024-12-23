using AEPOS.PriceChecker_11.SQLiteClasses.Models;
using SQLite;
using Sypram.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AEPOS.PriceChecker_11.SQLiteClasses
{
    public class CacheDataRepo : SypErrorBase
    {
        public const SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache;

        private const string DbName = "AdvTrack.db";
        private static SQLiteAsyncConnection _dbConn;
        private static readonly string DatabasePath = Path.Combine(FileSystem.AppDataDirectory, DbName);

        protected SQLiteAsyncConnection Database => _dbConn;

        // Public constructor
        static CacheDataRepo()
        {
            if (_dbConn == null)
            {
                _dbConn = new SQLiteAsyncConnection(DatabasePath, Flags);

                SQLiteAsyncConnection sQLiteAsync = _dbConn;
                sQLiteAsync.CreateTableAsync<StoreInfo>();
            }
        }

        // Asynchronous initialization method
        public async Task InitializeDatabaseAsync()
        {
            try
            {
                Console.WriteLine($"Initializing database at path: {DatabasePath}");
                await _dbConn.CreateTableAsync<StoreInfo>();
                Console.WriteLine("Database and tables initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
                throw new InvalidOperationException("Failed to initialize database tables.", ex);
            }
        }

        public async Task<List<T>> ExecuteSelectQueryAsync<T>(string query) where T : new()
        {
            try
            {
                var results = await Database.QueryAsync<T>(query);
                Debug.WriteLine(results);
                return results.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while executing query: {ex.Message}");
                return new List<T>();
            }
        }

        public async Task<StoreInfo> LoadStore()
        {
            ResetErrors();

            try
            {
                var results = await Database.QueryAsync<StoreInfo>($"SELECT * from StoreInfo");
                if (results.Count > 0)
                {
                    return results[0];
                }

                Console.WriteLine("Store data not found in local");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while executing query: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> SaveStoreInfoData()
        {
            ResetErrors();

            try
            {
                await Database.ExecuteAsync("DELETE FROM STOREINFO");

                string insertQuery = "INSERT INTO STOREINFO (StoreGUID,ConnectionString,StoreName,City,Location,STDateFormat,StoreID) VALUES (?,?,?,?,?,?,?) ";
                await Database.ExecuteAsync(insertQuery, Globals.StoreREC.STORE_GUID, Globals.GetServer_ConnStr, Globals.StoreREC.StoreName, Globals.StoreREC.City, Globals.StoreREC.LOCATION, Globals.StrDateFormat, Globals.StoreREC.StoreID);

                return true;
            }
            catch (Exception ex)
            {
                AddError("Error occured while saving STOREINFO data : " + ex.Message, this + ": SaveStoreInfoData");
                return false;
            }
        }

        public async Task<bool> RemoveStoreData()
        {
            ResetErrors();

            try
            {
                await Database.ExecuteAsync("DELETE FROM STOREINFO");


                return true;
            }
            catch (Exception ex)
            {
                AddError("Error occured while removing STOREINFO data : " + ex.Message, this + ": SaveStoreInfoData");
                return false;
            }
        }

    }
}
