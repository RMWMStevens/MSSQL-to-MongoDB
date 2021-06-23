using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Models;
using MSSQL_to_MongoDB.Models.MSSQL;

namespace MSSQL_to_MongoDB.Services
{
    public class MongoDbService : DatabaseService
    {
        public MongoDbService()
        {
            system = Models.Enums.DatabaseSystem.MongoDB;
        }

        public override string GetExampleFormat()
        {
            return @"mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb";
        }

        public ActionResult Export(MSSQL database)
        {
            try
            {
                return new ActionResult { IsSuccess = true };
            }
            catch (System.Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<string>(ex);
            }
        }
    }
}
