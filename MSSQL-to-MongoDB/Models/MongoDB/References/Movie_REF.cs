using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Models.MongoDB.References
{
    public class Movie_REF
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public string Age { get; set; }
        public string MediaType { get; set; }
        public int Runtime { get; set; }
        public List<ObjectId> ReleasedInCountries { get; set; }
        public List<MovieRating> Ratings { get; set; }
        public List<string> Platforms { get; set; }
    }
}
