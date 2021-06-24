using MongoDB.Driver;
using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Models;
using MSSQL_to_MongoDB.Models.MongoDB;
using MSSQL_to_MongoDB.Models.MongoDB.Enums;
using System;
using System.Collections.Generic;

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

        public ActionResult Export(MONGO_DB mongoDb)
        {
            try
            {
                Insert(Collections.MOVIES, mongoDb.Movies);

                //InsertOne(Collections.USERS, sqlDatabase.ToUsers().ToBsonDocument());
                //InsertOne(Collections.MOVIES, movies.ToBsonDocument());
                //InsertOne(Collections.PLATFORMS, sqlDatabase.ToPlatforms().ToBsonDocument());

                return new ActionResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<string>(ex);
            }
        }

        private void Insert<T>(Collections collectionName, T data)
        {
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase(databaseName);
            var collection = database.GetCollection<T>(collectionName.ToString());
            collection.InsertOne(data);
        }

        private void Insert<T>(Collections collectionName, List<T> list)
        {
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase(databaseName);
            var collection = database.GetCollection<T>(collectionName.ToString());
            collection.InsertMany(list);
        }
    }
}
