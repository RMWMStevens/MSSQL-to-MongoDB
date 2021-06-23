using MSSQL_to_MongoDB.Models;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MSSQL_to_MongoDB.Helpers
{
    public static class FileHelper
    {
        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static bool FileEmpty(string filePath)
        {
            try
            {
                var result = File.ReadAllBytes(filePath).Length <= 0;
                return File.ReadAllBytes(filePath).Length <= 0;
            }
            catch
            {
                return true;
            }
        }

        public static ActionResult SaveFile<T>(string filePath, T connectionInfo)
        {
            try
            {
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

        public static ActionResult<T> LoadFile<T>(string filePath)
        {
            try
            {
                if (!FileExists(filePath)) { return ActionResultHelper.CreateFailureResult<T>("File does not exist!"); }
                if (FileEmpty(filePath)) { return ActionResultHelper.CreateFailureResult<T>("File is empty!"); }

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
