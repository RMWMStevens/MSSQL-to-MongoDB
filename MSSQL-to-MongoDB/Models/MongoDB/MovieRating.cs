using MongoDB.Bson.Serialization.Attributes;

namespace MSSQL_to_MongoDB.Models.MongoDB
{
    public class MovieRating
    {
        public string RatingSite { get; set; }
        public int Rating { get; set; }
    }
}