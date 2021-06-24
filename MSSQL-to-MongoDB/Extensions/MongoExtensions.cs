using MSSQL_to_MongoDB.Models.MongoDB;
using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Extensions
{
    public static class MongoExtensions
    {
        public static Movie ToMovie(this string movieRowString, List<string> ratingRowStrings, List<string> countryRowStrings)
        {
            var movieRowCols = movieRowString.Split('|');

            return new Movie
            {
                Title = movieRowCols[0],
                Age = movieRowCols[1],
                MediaType = movieRowCols[2],
                Runtime = int.Parse(movieRowCols[3]),
                Ratings = ratingRowStrings.ToRatings(),
                ReleasedInCountries = countryRowStrings.ToCountries()
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
    }
}
