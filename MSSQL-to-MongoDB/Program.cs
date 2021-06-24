using MSSQL_to_MongoDB.Services;
using System;

namespace MSSQL_to_MongoDB
{
    class Program
    {
        private static readonly SqlService sqlService = new SqlService();
        private static readonly MongoService mongoService = new MongoService();

        static void Main(string[] args)
        {
            sqlService.LoadOnStartup();
            mongoService.LoadOnStartup();
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
            Console.WriteLine("1) Start conversion from MSSQL to MongoDB");
            Console.WriteLine("2) Change MSSQL connection string");
            Console.WriteLine("3) Change MongoDB connection string");
            Console.WriteLine("4) Show current connection info");
            Console.WriteLine("ESC) Exit");

            var key = Console.ReadKey().Key;
            Console.Clear();

            switch (key)
            {
                case ConsoleKey.D1:
                    Console.WriteLine("Importing SQL database to MongoDB schema...");
                    //var importResult = sqlService.ImportDatabase();
                    var importResult = sqlService.ImportToMongoScheme();

                    if (!importResult.IsSuccess)
                    {
                        Console.WriteLine($"Importing failed: {importResult.Message}");
                        PressToContinue();
                        return false;
                    }

                    var mongoDb = importResult.Data;
                    Console.WriteLine("Succesfully imported");
                    Console.WriteLine("Converting and exporting to MongoDB...");
                    var exportResult = mongoService.ExportPrimaries(mongoDb);

                    if (!exportResult.IsSuccess)
                    {
                        Console.WriteLine($"Converting and exporting failed: {exportResult.Message}");
                        PressToContinue();
                        return false;
                    }

                    Console.WriteLine("Succesfully converted and exported");

                    PressToContinue();
                    return true;
                case ConsoleKey.D2:
                    sqlService.SetConnectionString();
                    return true;
                case ConsoleKey.D3:
                    mongoService.SetConnectionString();
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
            sqlService.ShowConnectionInfo();
            Console.WriteLine();
            mongoService.ShowConnectionInfo();
        }

        static void PressToContinue()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
