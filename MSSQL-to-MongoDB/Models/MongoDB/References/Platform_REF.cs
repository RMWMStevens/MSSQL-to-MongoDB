using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Models.MongoDB.References
{
    public class Platform_REF
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string PlatformName { get; set; }
        public List<ObjectId> Movies { get; set; }
    }
}
