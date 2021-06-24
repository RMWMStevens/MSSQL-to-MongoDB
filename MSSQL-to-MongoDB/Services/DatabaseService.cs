using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Models.Enums;
using System;

namespace MSSQL_to_MongoDB.Services
{
    public abstract class DatabaseService
    {
        protected string connectionString;
        public string ConnectionString { get { return connectionString; } }

        protected DatabaseSystem system;
        public DatabaseSystem System { get { return system; } }

        public string GetFilePath(DatabaseSystem system)
        {
            return $"./MSSQL-to-MongoDB - Settings/mssql-to-mongodb_{system}.bin";
        }

        public void ShowConnectionInfo()
        {
            Console.WriteLine($"Current MSSQL connection string: \n{connectionString}");
        }

        public void LoadOnStartup()
        {
            LogHelper.Log($"Reading {system} configuration file...");

            var loadResult = FileHelper.LoadFile<string>(GetFilePath(system));
            if (!loadResult.IsSuccess)
            {
                Console.WriteLine(loadResult.Message);
                return;
            }

            LogHelper.Log($"Loaded {system} connection strings succesfully from local filesystem");
            connectionString = loadResult.Data;
        }

        public void SetConnectionString()
        {
            try
            {
                Console.WriteLine($"Setting connection string for database system: {system}");
                Console.WriteLine($"Leave empty and press Enter to skip setting a new string\n");
                Console.WriteLine($"The connection string should be of the following format: \n{GetExampleFormat()}\n");

                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) { return; }

                connectionString = input;
                FileHelper.SaveFile(GetFilePath(System), connectionString);
            }
            catch (Exception ex)
            {
                LogHelper.Log($"Setting new connection string went wrong: {ex.Message}");
            }
        }

        public abstract string GetExampleFormat();
    }
}
