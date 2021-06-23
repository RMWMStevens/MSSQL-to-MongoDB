using MSSQL_to_MongoDB.Models;
using System;

namespace MSSQL_to_MongoDB.Helpers
{
    public static class ActionResultHelper
    {
        public static ActionResult<T> CreateSuccessResult<T>(T data, string message = default)
        {
            return new ActionResult<T>()
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };
        }

        public static ActionResult<T> CreateErrorResult<T>(Exception ex)
        {
            return new ActionResult<T>()
            {
                IsSuccess = false,
                Message = ex.Message,
                Data = default
            };
        }
    }
}
