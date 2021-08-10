using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Models.Enums;
using System;
using System.Threading.Tasks;

namespace MSSQL_to_MongoDB.Services
{
    public abstract class BaseDbService
    {
        protected string connectionString;
        public string ConnectionString { get { return connectionString; } }

        protected DatabaseSystem system;
        public DatabaseSystem System { get { return system; } }

        public string GetFilePath(DatabaseSystem system)
        {
            return $"./MSSQL-to-MongoDB - Settings/mssql-to-mongodb_{system}.txt";
        }

        public void ShowConnectionInfo()
        {
            Console.WriteLine($"Current {system} connection string: \n{connectionString}");
        }

        public async Task LoadConfigFromFileSystemAsync()
        {
            LogHelper.Log($"Reading {system} configuration file...");

            var loadResult = await FileHelper.LoadAsync<string>(GetFilePath(system));
            if (!loadResult.IsSuccess)
            {
                LogHelper.LogError(loadResult.Message);
                return;
            }

            LogHelper.Log($"Loaded {system} connection strings successfully from local filesystem");
            connectionString = loadResult.Data;
        }

        public void SetConnectionString()
        {
            try
            {
                Console.WriteLine($"Setting connection string for database system: {system}");
                Console.WriteLine($"Leave empty and press Enter to skip setting a new string\n");
                Console.WriteLine($"The connection string should be of the following format: \n{GetExampleConnectionStringFormat()}\n");

                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) { return; }

                connectionString = input;
                FileHelper.SaveAsync(GetFilePath(System), connectionString);
            }
            catch (Exception ex)
            {
                LogHelper.Log($"Setting new connection string went wrong: {ex.Message}");
            }
        }

        public abstract string GetExampleConnectionStringFormat();
    }
}
