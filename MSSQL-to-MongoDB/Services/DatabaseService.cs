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
            return $"./mssql-to-mongodb_{system}.bin";
        }

        public void ShowConnectionInfo()
        {
            Console.WriteLine($"Current MS SQL connection string: \n{connectionString}");
        }

        public abstract string GetExampleFormat();

        public void LoadOnStartup()
        {
            Console.WriteLine($"Reading {system} configuration file...");

            var loadResult = FileHelper.LoadFile<string>(GetFilePath(system));
            if (!loadResult.IsSuccess)
            {
                Console.WriteLine(loadResult.Message);
                return;
            }

            Console.WriteLine($"Loaded {system} connection strings succesfully from local filesystem");
            connectionString = loadResult.Data;
        }

        public void SetConnectionString()
        {
            try
            {
                Console.WriteLine($"Setting connection string for database system: {system}\n");
                Console.WriteLine($"The connection string should be of the following format: \n{GetExampleFormat()}\n");

                connectionString = Console.ReadLine();
                FileHelper.SaveFile(GetFilePath(System), connectionString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Setting new connection string went wrong: {ex.Message}");
            }
        }
    }
}
