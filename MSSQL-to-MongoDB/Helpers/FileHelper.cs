﻿using MSSQL_to_MongoDB.Models;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace MSSQL_to_MongoDB.Helpers
{
    public static class FileHelper
    {
        public static bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static async Task<bool> IsEmptyAsync(string filePath)
        {
            try
            {
                return (await File.ReadAllBytesAsync(filePath)).Length <= 0;
            }
            catch
            {
                return true;
            }
        }

        public static ActionResult Save<T>(string filePath, T connectionInfo)
        {
            try
            {
                new FileInfo(filePath).Directory.Create();
                using Stream stream = File.Open(filePath, FileMode.Create);
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, connectionInfo);
                return new ActionResult { IsSuccess = true };
            }
            catch (System.Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<T>(ex);
            }
        }

        public static async Task<ActionResult<T>> LoadAsync<T>(string filePath)
        {
            try
            {
                if (!Exists(filePath)) { return ActionResultHelper.CreateFailureResult<T>("File does not exist!"); }
                if (await IsEmptyAsync(filePath)) { return ActionResultHelper.CreateFailureResult<T>("File is empty!"); }

                using Stream stream = File.Open(filePath, FileMode.OpenOrCreate);
                var binaryFormatter = new BinaryFormatter();
                var result = (T)binaryFormatter.Deserialize(stream);
                return ActionResultHelper.CreateSuccessResult(result);
            }
            catch (System.Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<T>(ex);
            }
        }
    }
}
