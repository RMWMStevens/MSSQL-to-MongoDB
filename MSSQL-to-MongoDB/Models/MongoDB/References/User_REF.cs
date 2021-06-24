using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Models.MongoDB.References
{
    public class User_REF
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public ObjectId Country { get; set; }
        public string Sex { get; set; }
        public List<string> Platforms { get; set; }
        public List<string> MediaTypes { get; set; }
        public List<ObjectId> Favorites { get; set; }
    }
}
