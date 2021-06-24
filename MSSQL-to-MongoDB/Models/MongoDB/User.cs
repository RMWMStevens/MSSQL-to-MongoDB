using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Models.MongoDB
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        [BsonIgnore]
        public string CountryCode { get; set; }
        public string Sex { get; set; }
        [BsonIgnore]
        public List<Platform> Platforms { get; set; }
        public List<string> MediaTypes { get; set; }
        [BsonIgnore]
        public List<Movie> Favorites { get; set; }
    }
}
