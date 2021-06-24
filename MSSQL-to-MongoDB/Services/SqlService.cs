using MSSQL_to_MongoDB.Extensions;
using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Models;
using MSSQL_to_MongoDB.Models.MongoDB;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace MSSQL_to_MongoDB.Services
{
    public class SqlService : DatabaseService
    {
        public SqlService()
        {
            system = Models.Enums.DatabaseSystem.MSSQL;
        }

        public override string GetExampleFormat()
        {
            var sqlAuth = @"Data Source=COMPUTERNAME;Initial Catalog=DATABASENAME;User ID=USERNAME;Password=PASSWORD";
            var winAuth = @"Data Source=COMPUTERNAME;Initial Catalog=DATABASENAME;Integrated Security=SSPI;";

            return $"For SQL Authentication: \n{sqlAuth}\nFor Windows Authentication (leave 'SSPI' as is): \n{winAuth}";
        }

        public ActionResult<MONGO_DB> ImportToMongoScheme()
        {
            try
            {
                var mongoDb = new MONGO_DB
                {
                    Countries = ImportCountriesToMongoScheme(),
                    Movies = ImportMoviesToMongoScheme(),
                    Platforms = ImportPlatformsToMongoScheme(),
                    Users = ImportUsersToMongoScheme(),
                };
                return ActionResultHelper.CreateSuccessResult(mongoDb);
            }
            catch (Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<MONGO_DB>(ex);
            }
        }

        private List<Country> ImportCountriesToMongoScheme()
        {
            var countryCodes = RunQuery("SELECT CountryCode FROM COUNTRIES ORDER BY 1");

            var countries = new List<Country>();

            foreach (var countryCode in countryCodes)
            {
                var countryName = RunQuery($"SELECT Country FROM COUNTRIES WHERE CountryCode = '{countryCode}'").First();

                countries.Add(new Country
                {
                    CountryCode = countryCode,
                    CountryName = countryName
                });
            }

            return countries;
        }

        private List<Movie> ImportMoviesToMongoScheme()
        {
            var movieIDs = RunQuery("SELECT MovieID FROM MOVIES ORDER BY 1").Select(int.Parse).ToList();

            var movies = new List<Movie>();

            foreach (var movieId in movieIDs)
            {
                var movieRowString = RunQuery($"SELECT Title, Age, MediaType, Runtime FROM MOVIES WHERE MovieID = {movieId} ORDER BY MovieID").First();
                var ratingRowStrings = RunQuery($"SELECT RatingSite, Rating FROM MOVIE_RATINGS WHERE MovieID = {movieId}");
                //var countryRowStrings = RunQuery($@"SELECT C.CountryCode, C.Country FROM MOVIE_IN_COUNTRIES MC
                //                                    INNER JOIN COUNTRIES C ON C.CountryCode = MC.CountryCode
                //                                    WHERE MovieID = {movieId}");

                movies.Add(movieRowString.ToMovie(ratingRowStrings));
            }

            return movies;
        }

        private List<Platform> ImportPlatformsToMongoScheme()
        {
            var platformNames = RunQuery("SELECT Platform FROM PLATFORMS ORDER BY 1");
            return platformNames.ToPlatforms();
        }

        private List<User> ImportUsersToMongoScheme()
        {
            var userIDs = RunQuery("SELECT UserID FROM USERS ORDER BY 1").Select(int.Parse).ToList();

            var users = new List<User>();

            foreach(var userId in userIDs)
            {
                var userRowString = RunQuery($"SELECT FullName, Email, BirthDate, CountryCode, Sex FROM USERS WHERE UserID = {userId}").First();
                var mediaTypeRowStrings = RunQuery($"SELECT MediaType FROM USER_MEDIA_TYPES WHERE UserID = {userId}");

                users.Add(userRowString.ToUser(mediaTypeRowStrings));
            }

            return users;
        }

        public List<string> RunQuery(string sqlQuery)
        {
            var sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            var command = new SqlCommand(sqlQuery, sqlConnection);
            var dataReader = command.ExecuteReader();

            var rows = new List<string>();

            while (dataReader.Read())
            {
                var row = string.Empty;

                for (var i = 0; i < dataReader.FieldCount; i++)
                {
                    if (i != 0) { row += "|"; }
                    row += dataReader.GetValue(i);
                }
                rows.Add(row);
            }

            sqlConnection.Close();
            return rows;
        }
    }
}
