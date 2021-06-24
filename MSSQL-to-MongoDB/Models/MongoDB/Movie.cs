using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Models.MongoDB
{
    public class Movie
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int MovieID { get; set; }
        public string Title { get; set; }
        public string Age { get; set; }
        public string MediaType { get; set; }
        public int Runtime { get; set; }
        [BsonIgnore]
        public List<Country> ReleasedInCountries { get; set; }
        [BsonIgnore]
        public List<MovieRating> Ratings { get; set; }
        public List<string> Platforms { get; set; }
    }
}
