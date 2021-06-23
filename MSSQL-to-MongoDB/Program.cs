using MSSQL_to_MongoDB.Models;
using MSSQL_to_MongoDB.Models.Enums;
using MSSQL_to_MongoDB.Services;
using System;
//using SqlModels = MSSQL_to_MongoDB.Models.MSSQL;
//using MongoModels = MSSQL_to_MongoDB.Models.MongoDB;

namespace MSSQL_to_MongoDB
{
    class Program
    {
        private static readonly string filePath = "./mssql-to-mongodb-conn.bin";
        private static ConnectionInfo connectionInfo;

        private static FileService fileService = new FileService();

        static void Main(string[] args)
        {
            connectionInfo = fileService.LoadOnStartup(filePath);
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

            var key = Console.ReadKey().Key;
            Console.Clear();

            switch (key)
            {
                case ConsoleKey.D1:
                    return true;
                case ConsoleKey.D2:
                    var sqlResult = fileService.SetConnectionString(connectionInfo, filePath, DatabaseSystem.MSSQL);
                    if (!sqlResult.IsSuccess) { Console.WriteLine($"Something went wrong: {sqlResult.Message}"); }
                    return true;
                case ConsoleKey.D3:
                    var mongoResult = fileService.SetConnectionString(connectionInfo, filePath, DatabaseSystem.MongoDB);
                    if (!mongoResult.IsSuccess) { Console.WriteLine($"Something went wrong: {mongoResult.Message}"); }
                    return true;
                case ConsoleKey.D4:
                    ShowConnectionInfo();
                    PressToContinue();
                    return true;
                case ConsoleKey.Escape:
                    return false;
                default:
                    return true;
            }
        }

        static void ShowConnectionInfo()
        {
            Console.WriteLine($"Current MS SQL connection string: \n{connectionInfo.MSSQL}");
            Console.WriteLine();
            Console.WriteLine($"Current MongoDB connection string: \n{connectionInfo.MongoDB}");
        }

        static void PressToContinue()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
