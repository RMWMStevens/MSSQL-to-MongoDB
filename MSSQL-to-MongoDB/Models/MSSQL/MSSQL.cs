using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Models.MSSQL
{
    public class MSSQL
    {
        public List<Country> Countries { get; set; }
        public List<FavoriteMoviePerUser> FavoriteMoviesPerUser { get; set; }
        public List<Movie> Movies { get; set; }
        public List<MovieInCountry> MovieInCountries { get; set; }
        public List<MovieOnPlatform> MovieOnPlatforms { get; set; }
        public List<MovieRating> MovieRatings { get; set; }
        public List<PlatformUser> PlatformUsers { get; set; }
        public List<User> Users { get; set; }
        public List<UserMediaType> UserMediaTypes { get; set; }
    }
}
