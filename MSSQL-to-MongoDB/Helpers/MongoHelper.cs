using MongoDB.Bson;
using MongoDB.Driver;
using MSSQL_to_MongoDB.Models.MongoDB.Enums;

namespace MSSQL_to_MongoDB.Helpers
{
    public static class MongoHelper
    {
        public static void Insert(string connectionString, string databaseName, Collections collectionName, BsonDocument bsonDocument)
        {
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase(databaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName.ToString());
            collection.InsertOne(bsonDocument);
        }
    }
}
