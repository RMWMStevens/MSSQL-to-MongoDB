using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Models;
using System;
//using SqlModels = MSSQL_to_MongoDB.Models.MSSQL;
//using MongoModels = MSSQL_to_MongoDB.Models.MongoDB;

namespace MSSQL_to_MongoDB
{
    class Program
    {
        private static readonly string filePath = "./mssql-to-mongodb-conn.bin";
        private static ConnectionStringInfo connectionStringInfo;

        static void Main(string[] args)
        {
            LoadOnStartup();
            PressToContinue();

            bool showMenu = true;
            while (showMenu)
            {
                showMenu = MainMenu();
            }
        }

        static bool MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Select an option:");
            Console.WriteLine("1) Start conversion from MS SQL to MongoDB");
            Console.WriteLine("2) Change MS SQL connection string");
            Console.WriteLine("3) Change MongoDB connection string");
            Console.WriteLine("4) Show current connection info");
            Console.WriteLine("ESC) Exit");

            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.D1:
                    return true;
                case ConsoleKey.D2:
                    ShowNotImplemented();
                    return true;
                case ConsoleKey.D3:
                    ShowNotImplemented();
                    return true;
                case ConsoleKey.D4:
                    ShowConnectionInfo();
                    return true;
                case ConsoleKey.Escape:
                    return false;
                default:
                    return true;
            }
        }

        static void LoadOnStartup()
        {
            Console.WriteLine("Reading connection strings configuration file...");

            var loadResult = FileHelper.LoadFile<ConnectionStringInfo>(filePath);
            if (!loadResult.IsSuccess)
            {
                Console.WriteLine(loadResult.Message);
                connectionStringInfo = new ConnectionStringInfo();
                return;
            }

            connectionStringInfo = loadResult.Data;
            Console.WriteLine("Loaded connection strings succesfully from local file system");
        }

        static void ShowConnectionInfo()
        {
            Console.WriteLine("\n");
            Console.WriteLine($"Current MS SQL connection string: {connectionStringInfo.MSSQL}");
            Console.WriteLine($"Current MongoDB connection string: {connectionStringInfo.MongoDB}");
            PressToContinue();
        }

        static void PressToContinue()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static void ShowNotImplemented()
        {
            Console.WriteLine("\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("This has not been implemented yet!");
            Console.ResetColor();
            PressToContinue();
        }
    }
}
