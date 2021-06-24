using MongoDB.Driver;
using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Models;
using MSSQL_to_MongoDB.Models.MongoDB;
using MSSQL_to_MongoDB.Models.MongoDB.Enums;
using MSSQL_to_MongoDB.Models.MongoDB.References;
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

        public ActionResult ExportPrimaries(MONGO_DB mongoDb)
        {
            try
            {
                DropCollections();

                Insert(Collections.COUNTRIES, mongoDb.Countries);
                Insert(Collections.PLATFORMS, mongoDb.Platforms);
                Insert(Collections.MOVIES, mongoDb.Movies);
                Insert(Collections.USERS, mongoDb.Users);

                return new ActionResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<string>(ex);
            }
        }

        public ActionResult ExportReferences(MONGO_DB_REF mongoDbRef)
        {
            try
            {
                // Insert(Collections.COLLECTION, mongoDb.ATTRIBUTE);

                return new ActionResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<string>(ex);
            }
        }

        private void Insert<T>(Collections collectionName, List<T> list)
        {
            Console.WriteLine($"MongoService - Exporting {collectionName}");
            var database = GetDatabase();
            var collection = database.GetCollection<T>(collectionName.ToString());
            collection.InsertMany(list);
        }

        private void DropCollections()
        {
            Console.WriteLine("MongoService - Dropping collections");
            var database = GetDatabase();
            var collections = (Collections[])Enum.GetValues(typeof(Collections));
            foreach (var collection in collections)
            {
                database.DropCollection(collection.ToString());
            }
        }

        private IMongoDatabase GetDatabase()
        {
            var mongoClient = new MongoClient(connectionString);
            return mongoClient.GetDatabase(databaseName);
        }
    }
}
