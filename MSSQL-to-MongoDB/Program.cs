using MSSQL_to_MongoDB.Services;
using System;

namespace MSSQL_to_MongoDB
{
    class Program
    {
        private static readonly MsSqlService msSqlService = new MsSqlService();
        private static readonly MongoDbService mongoDbService = new MongoDbService();

        private static bool completed = false;

        static void Main(string[] args)
        {
            msSqlService.LoadOnStartup();
            mongoDbService.LoadOnStartup();
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
                    if (completed) 
                    {
                        Console.WriteLine("Import already complete, it is not advisable to run it again");
                        PressToContinue();
                        return false;
                    }

                    Console.WriteLine("Importing from MSSQL...");
                    var importResult = msSqlService.Import();

                    if (!importResult.IsSuccess)
                    {
                        Console.WriteLine($"Import failed: {importResult.Message}");
                        PressToContinue();
                        return false;
                    }

                    Console.WriteLine("Import succesful");
                    Console.WriteLine("Exporting to MongoDB...");
                    var exportResult = mongoDbService.Export(importResult.Data);

                    if (!exportResult.IsSuccess)
                    {
                        Console.WriteLine($"Import failed: {exportResult.Message}");
                        PressToContinue();
                        return false;
                    }

                    Console.WriteLine("Export succesful");
                    completed = true;
                    PressToContinue();
                    return true;
                case ConsoleKey.D2:
                    msSqlService.SetConnectionString();
                    return true;
                case ConsoleKey.D3:
                    mongoDbService.SetConnectionString();
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
            msSqlService.ShowConnectionInfo();
            Console.WriteLine();
            mongoDbService.ShowConnectionInfo();
        }

        static void PressToContinue()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
