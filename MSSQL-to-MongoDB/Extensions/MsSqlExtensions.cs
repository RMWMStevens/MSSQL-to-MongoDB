using MSSQL_to_MongoDB.Models.MSSQL;
using System;
using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Extensions
{
    public static class MsSqlExtensions
    {
        public static List<Country> ToCountries(this List<string> rowStrings)
        {
            var countries = new List<Country>();

            foreach (var rowString in rowStrings)
            {
                countries.Add(rowString.ToCountry());
            }

            return countries;
        }

        public static Country ToCountry(this string rowString)
        {
            var rowCols = rowString.Split('|');

            return new Country
            {
                CountryCode = rowCols[0],
                CountryName = rowCols[1]
            };
        }

        public static List<FavoriteMoviePerUser> ToFavoriteMoviesPerUser(this List<string> rowStrings)
        {
            var favoriteMovies = new List<FavoriteMoviePerUser>();

            foreach (var rowString in rowStrings)
            {
                favoriteMovies.Add(rowString.ToFavoriteMoviePerUser());
            }

            return favoriteMovies;
        }

        public static FavoriteMoviePerUser ToFavoriteMoviePerUser(this string rowString)
        {
            var rowCols = rowString.Split('|');

            return new FavoriteMoviePerUser
            {
                UserID = int.Parse(rowCols[0]),
                MovieID = int.Parse(rowCols[1])
            };
        }

        public static List<Movie> ToMovies(this List<string> rowStrings)
        {
            var movies = new List<Movie>();

            foreach (var rowString in rowStrings)
            {
                movies.Add(rowString.ToMovie());
            }

            return movies;
        }

        public static Movie ToMovie(this string rowString)
        {
            var rowCols = rowString.Split('|');

            return new Movie
            {
                MovieID = int.Parse(rowCols[0]),
                Title = rowCols[1],
                Age = rowCols[2],
                MediaType = rowCols[3],
                Runtime = int.Parse(rowCols[4])
            };
        }

        public static List<MovieInCountry> ToMovieInCountries(this List<string> rowStrings)
        {
            var movies = new List<MovieInCountry>();

            foreach (var rowString in rowStrings)
            {
                movies.Add(rowString.ToMovieInCountry());
            }

            return movies;
        }

        public static MovieInCountry ToMovieInCountry(this string rowString)
        {
            var rowCols = rowString.Split('|');

            return new MovieInCountry
            {
                MovieID = int.Parse(rowCols[0]),
                CountryCode = rowCols[1]
            };
        }

        public static List<MovieOnPlatform> ToMovieOnPlatforms(this List<string> rowStrings)
        {
            var movies = new List<MovieOnPlatform>();

            foreach (var rowString in rowStrings)
            {
                movies.Add(rowString.ToMovieOnPlatform());
            }

            return movies;
        }

        public static MovieOnPlatform ToMovieOnPlatform(this string rowString)
        {
            var rowCols = rowString.Split('|');

            return new MovieOnPlatform
            {
                MovieID = int.Parse(rowCols[0]),
                Platform = rowCols[1]
            };
        }

        public static List<MovieRating> ToMovieRatings(this List<string> rowStrings)
        {
            var movies = new List<MovieRating>();

            foreach (var rowString in rowStrings)
            {
                movies.Add(rowString.ToMovieRating());
            }

            return movies;
        }

        public static MovieRating ToMovieRating(this string rowString)
        {
            var rowCols = rowString.Split('|');

            return new MovieRating
            {
                MovieID = int.Parse(rowCols[0]),
                RatingSite = rowCols[1],
                Rating = int.Parse(rowCols[2])
            };
        }

        public static List<PlatformUser> ToPlatformUsers(this List<string> rowStrings)
        {
            var movies = new List<PlatformUser>();

            foreach (var rowString in rowStrings)
            {
                movies.Add(rowString.ToPlatformUser());
            }

            return movies;
        }

        public static PlatformUser ToPlatformUser(this string rowString)
        {
            var rowCols = rowString.Split('|');

            return new PlatformUser
            {
                UserID = int.Parse(rowCols[0]),
                Platform = rowCols[1]
            };
        }

        public static List<User> ToUsers(this List<string> rowStrings)
        {
            var movies = new List<User>();

            foreach (var rowString in rowStrings)
            {
                movies.Add(rowString.ToUser());
            }

            return movies;
        }

        public static User ToUser(this string rowString)
        {
            var rowCols = rowString.Split('|');

            return new User
            {
                UserID = int.Parse(rowCols[0]),
                FullName = rowCols[1],
                Email = rowCols[2],
                BirthDate = DateTime.Parse(rowCols[3]),
                CountryCode = rowCols[4],
                Sex = rowCols[5]
            };
        }

        public static List<UserMediaType> ToUserMediaTypes(this List<string> rowStrings)
        {
            var movies = new List<UserMediaType>();

            foreach (var rowString in rowStrings)
            {
                movies.Add(rowString.ToUserMediaType());
            }

            return movies;
        }

        public static UserMediaType ToUserMediaType(this string rowString)
        {
            var rowCols = rowString.Split('|');

            return new UserMediaType
            {
                UserID = int.Parse(rowCols[0]),
                MediaType = rowCols[1]
            };
        }
    }
}
