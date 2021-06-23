using MSSQL_to_MongoDB.Models.MongoDB;
using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Extensions
{
    public static class SqlToMongoExtensions
    {
        public static List<Movie> ToMovies(this Models.MSSQL.MSSQL sqlDatabase)
        {
            var movies = new List<Movie>();

            foreach (var movie in sqlDatabase.Movies)
            {
                movies.Add(movie.ToMovie(sqlDatabase));
            }

            return movies;
        }

        public static Movie ToMovie(this Models.MSSQL.Movie sqlMovie, Models.MSSQL.MSSQL sqlDatabase)
        {
            var movie = new Movie
            {
                Title = sqlMovie.Title,
                Age = sqlMovie.Age,
                MediaType = sqlMovie.MediaType,
                Runtime = sqlMovie.Runtime,
                Ratings = new List<MovieRating>(),
                ReleasedInCountries = new List<Country>()
            };

            var sqlMovieRatings = sqlDatabase.MovieRatings.FindAll(m => m.MovieID == sqlMovie.MovieID);

            foreach(var sqlMovieRating in sqlMovieRatings)
            {
                movie.Ratings.Add(sqlMovieRating.ToMovieRating());
            }

            var sqlMovieInCountries = sqlDatabase.MovieInCountries.FindAll(m => m.MovieID == sqlMovie.MovieID);

            foreach(var sqlMovieInCountry in sqlMovieInCountries)
            {
                movie.ReleasedInCountries.Add(sqlMovieInCountry.ToCountry(sqlDatabase.Countries));
            }

            return movie;
        }

        public static MovieRating ToMovieRating(this Models.MSSQL.MovieRating sqlMovieRating)
        {
            return new MovieRating
            {
                RatingSite = sqlMovieRating.RatingSite,
                Rating = sqlMovieRating.Rating
            };
        }

        public static Country ToCountry(this Models.MSSQL.MovieInCountry sqlMovieInCountry, List<Models.MSSQL.Country> sqlCountries)
        {
            return new Country
            {
                CountryCode = sqlMovieInCountry.CountryCode,
                CountryName = sqlCountries.Find(c => c.CountryCode == sqlMovieInCountry.CountryCode).CountryName
            };
        }

        public static List<User> ToUsers(this Models.MSSQL.MSSQL sqlDatabase)
        {
            var users = new List<User>();

            return users;
        }

        public static List<Platform> ToPlatforms(this Models.MSSQL.MSSQL sqlDatabase)
        {
            var platforms = new List<Platform>();

            return platforms;
        }
    }
}
