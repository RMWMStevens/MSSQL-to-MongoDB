using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Models.MongoDB
{
    public class Movie
    {
        public string Title { get; set; }
        public string Age { get; set; }
        public string MediaType { get; set; }
        public int Runtime { get; set; }
        public List<Country> ReleasedInCountries { get; set; }
        public List<MovieRating> Ratings { get; set; }
    }
}
