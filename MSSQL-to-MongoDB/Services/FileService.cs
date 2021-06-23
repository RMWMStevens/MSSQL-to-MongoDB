using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Models;
using MSSQL_to_MongoDB.Models.Enums;
using System;

namespace MSSQL_to_MongoDB.Services
{
    public class FileService
    {
        public ConnectionInfo LoadOnStartup(string filePath)
        {
            Console.WriteLine("Reading connection strings configuration file...");

            var loadResult = FileHelper.LoadFile<ConnectionInfo>(filePath);
            if (!loadResult.IsSuccess)
            {
                Console.WriteLine(loadResult.Message);
                return new ConnectionInfo();
            }

            Console.WriteLine("Loaded connection strings succesfully from local file system");
            return loadResult.Data;
        }

        public ActionResult<ConnectionInfo> SetConnectionString(ConnectionInfo connectionInfo, string filePath, DatabaseSystem system)
        {
            try
            {
                Console.WriteLine($"Setting connection string for database system \n{system}\n");
                Console.WriteLine($"The connection string should be of the following format: \n{connectionInfo.GetExampleFormat(system)}\n");

                switch (system)
                {
                    case DatabaseSystem.MSSQL:
                        connectionInfo.MSSQL = Console.ReadLine();
                        break;
                    case DatabaseSystem.MongoDB:
                        connectionInfo.MongoDB = Console.ReadLine();
                        break;
                }

                FileHelper.SaveFile(filePath, connectionInfo);
                return ActionResultHelper.CreateSuccessResult(connectionInfo);
            }
            catch (Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<ConnectionInfo>(ex);
            }
        }
    }
}
