﻿using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MSSQL_to_MongoDB
{
    class Program
    {
        private static readonly SqlService sqlService = new SqlService();
        private static readonly MongoService mongoService = new MongoService();

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            await LoadOnStartup();

            bool showMenu = true;
            while (showMenu)
            {
                showMenu = await ShowMainMenu();
            }
        }

        static async Task<bool> ShowMainMenu()
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
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    LogHelper.Log("Importing SQL database to MongoDB schema...");
                    var importResult = await sqlService.ImportToMongoSchema();

                    if (!importResult.IsSuccess)
                    {
                        LogHelper.Log($"Importing failed: {importResult.Message}");
                        PressToContinue();
                        return false;
                    }

                    var mongoDb = importResult.Data;
                    LogHelper.Log("Successfully imported");

                    LogHelper.Log($"Import | Time: {stopwatch.Elapsed}");

                    LogHelper.Log("Converting and exporting to MongoDB...");
                    var exportResult = await mongoService.Export(mongoDb);

                    if (!exportResult.IsSuccess)
                    {
                        LogHelper.Log($"Converting and exporting failed: {exportResult.Message}");
                        PressToContinue();
                        return false;
                    }

                    LogHelper.Log("Successfully converted and exported");
                    LogHelper.Log($"Convert & Export | Total time: {stopwatch.Elapsed}");
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

        static async Task LoadOnStartup()
        {
            var tasks = new List<Task>
            {
                sqlService.LoadOnStartup(),
                mongoService.LoadOnStartup()
            };

            await Task.WhenAll(tasks);

            PressToContinue();
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
