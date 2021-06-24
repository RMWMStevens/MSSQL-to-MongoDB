using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MSSQL_to_MongoDB.Models.MongoDB
{
    public class Country
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
    }
}
