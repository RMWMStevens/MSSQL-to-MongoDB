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
        public Country Country { get; set; }
        public string Sex { get; set; }
        public IEnumerable<string> Platforms { get; set; }
        public IEnumerable<string> MediaTypes { get; set; }
        [BsonIgnore]
        public IEnumerable<Movie> Favorites { get; set; }
    }
}
