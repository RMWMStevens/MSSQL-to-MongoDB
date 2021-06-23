using MongoDB.Bson;
using MongoDB.Driver;
using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Models;
using MSSQL_to_MongoDB.Models.MongoDB.Enums;
using MSSQL_to_MongoDB.Models.MSSQL;
using MSSQL_to_MongoDB.Extensions;
using System;

namespace MSSQL_to_MongoDB.Services
{
    public class MongoService : DatabaseService
    {
        private const string databaseName = "What2Watch";

        public MongoService()
        {
            system = Models.Enums.DatabaseSystem.MongoDB;
        }

        public override string GetExampleFormat()
        {
            return @"mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb";
        }

        public ActionResult Export(MSSQL sqlDatabase)
        {
            try
            {
                var movies = sqlDatabase.ToMovies();
                //Insert(Collections.USERS, sqlDatabase.ToUsers().ToBsonDocument());
                MongoHelper.Insert(connectionString, databaseName, Collections.MOVIES, movies.ToBsonDocument());
                //Insert(Collections.PLATFORMS, sqlDatabase.ToPlatforms().ToBsonDocument());

                return new ActionResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<string>(ex);
            }
        }

        
    }
}
