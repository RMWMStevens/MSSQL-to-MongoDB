using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Models.MongoDB
{
    public class Platform
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string PlatformName { get; set; }
    }
}
