using MSSQL_to_MongoDB.Models.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSSQL_to_MongoDB.Extensions
{
    public static class MongoExtensions
    {
        public static List<Movie> ToMoviesWithoutRatingsCountriesPlatforms(this List<string> movieRowStrings)
        {
            var movies = new List<Movie>();

            var emptyList = new List<string>();

            foreach (var movieRowString in movieRowStrings)
            {
                movies.Add(movieRowString.ToMovie(emptyList, emptyList, emptyList));
            }

            return movies;
        }

        public static Movie ToMovie(this string movieRowString, List<string> ratingRowStrings, List<string> countryRowStrings, List<string> platformStrings)
        {
            var movieRowCols = movieRowString.Split('|');

            return new Movie
            {
                Title = movieRowCols[0],
                Age = movieRowCols[1],
                MediaType = movieRowCols[2],
                Runtime = int.Parse(movieRowCols[3]),
                Ratings = ratingRowStrings.ToRatings(),
                ReleasedInCountries = countryRowStrings.ToCountries(),
                MovieID = int.Parse(movieRowCols[4]),
                Platforms = platformStrings,
            };
        }

        public static List<MovieRating> ToRatings(this List<string> ratingRowStrings)
        {
            var ratings = new List<MovieRating>();

            foreach (var ratingRowString in ratingRowStrings)
            {
                ratings.Add(ratingRowString.ToRating());
            }

            return ratings;
        }

        public static MovieRating ToRating(this string ratingRowString)
        {
            var ratingRowCols = ratingRowString.Split('|');

            return new MovieRating
            {
                RatingSite = ratingRowCols[0],
                Rating = int.Parse(ratingRowCols[1])
            };
        }

        public static List<Country> ToCountries(this List<string> countryRowStrings)
        {
            var countries = new List<Country>();

            foreach (var countryRowString in countryRowStrings)
            {
                countries.Add(countryRowString.ToCountry());
            }

            return countries;
        }

        public static Country ToCountry(this string countryRowString)
        {
            var countryRowCols = countryRowString.Split('|');

            return new Country
            {
                CountryCode = countryRowCols[0],
                CountryName = countryRowCols[1]
            };
        }

        public static User ToUser(this string userRowString, List<string> favoriteMovieRowStrings, List<string> platformStrings, List<string> mediaTypeStrings)
        {
            var userRowCols = userRowString.Split('|');

            return new User
            {
                FullName = userRowCols[0],
                Email = userRowCols[1],
                BirthDate = DateTime.Parse(userRowCols[2]),
                CountryCode = userRowCols[3],
                Sex = userRowCols[4],
                MediaTypes = mediaTypeStrings.ToMediaTypes(),
                Platforms = platformStrings,
                Favorites = favoriteMovieRowStrings.ToMoviesWithoutRatingsCountriesPlatforms()
            };
        }

        public static List<string> ToMediaTypes(this List<string> mediaTypeStrings)
        {
            return mediaTypeStrings.SelectMany(t => t.Split('|')).ToList();
        }
    }
}
