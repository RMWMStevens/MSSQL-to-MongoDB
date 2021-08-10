using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MSSQL_to_MongoDB
{
    class Program
    {
        private static readonly MsSqlService msSqlService = new MsSqlService();
        private static readonly MongoService mongoService = new MongoService();

        static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            await LoadConfigFromFileSystemAsync();

            bool showMenu = true;
            while (showMenu)
            {
                showMenu = await ShowMenuAsync();
            }
        }

        static async Task<bool> ShowMenuAsync()
        {
            Console.Clear();
            Console.WriteLine("Select an option:");
            Console.WriteLine("i) Show current connection info");
            Console.WriteLine("Enter) Start conversion from MSSQL to MongoDB");
            Console.WriteLine("1) Change MSSQL connection string");
            Console.WriteLine("2) Change MongoDB connection string");
            Console.WriteLine("ESC) Exit");

            var key = Console.ReadKey().Key;
            Console.Clear();

            switch (key)
            {
                case ConsoleKey.I:
                    ShowConnectionInfo();
                    PressToContinue();
                    return true;
                case ConsoleKey.Enter:
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    LogHelper.Log("Importing SQL database to MongoDB schema...");
                    var importResult = await msSqlService.ImportToMongoSchemaAsync();

                    if (!importResult.IsSuccess)
                    {
                        LogHelper.LogError($"Importing failed: {importResult.Message}");
                        PressToContinue();
                        return false;
                    }

                    var mongoDb = importResult.Data;
                    LogHelper.Log("Successfully imported");

                    LogHelper.Log($"Import | Time: {stopwatch.Elapsed}");

                    LogHelper.Log("Converting and exporting to MongoDB...");
                    var exportResult = await mongoService.ExportAsync(mongoDb);

                    if (!exportResult.IsSuccess)
                    {
                        LogHelper.LogError($"Converting and exporting failed: {exportResult.Message}");
                        PressToContinue();
                        return false;
                    }

                    LogHelper.Log("Successfully converted and exported");
                    LogHelper.Log($"Convert & Export | Total time: {stopwatch.Elapsed}");
                    PressToContinue();
                    return true;
                case ConsoleKey.D1:
                    msSqlService.SetConnectionString();
                    return true;
                case ConsoleKey.D2:
                    mongoService.SetConnectionString();
                    return true;
                case ConsoleKey.Escape:
                    return false;
                default:
                    return true;
            }
        }

        static async Task LoadConfigFromFileSystemAsync()
        {
            await Task.WhenAll(
                msSqlService.LoadConfigFromFileSystemAsync(),
                mongoService.LoadConfigFromFileSystemAsync()
            );

            PressToContinue();
        }

        static void ShowConnectionInfo()
        {
            msSqlService.ShowConnectionInfo();
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
