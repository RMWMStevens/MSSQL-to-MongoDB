using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Models.MongoDB.References
{
    public class UserRef
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public Country Country { get; set; }
        public string Sex { get; set; }
        public IEnumerable<string> Platforms { get; set; }
        public IEnumerable<string> MediaTypes { get; set; }
        public IEnumerable<ObjectId> Favorites { get; set; }
    }
}
