using MSSQL_to_MongoDB.Extensions;
using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Models;
using MSSQL_to_MongoDB.Models.MongoDB;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
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
            LogHelper.Log("Importing COUNTRIES", nameof(SqlService));

            var countryRowStrings = RunQuery("SELECT CountryCode, Country FROM COUNTRIES ORDER BY 1");

            LogHelper.Log("Import complete", nameof(SqlService));

            return countryRowStrings.ToCountries();
        }

        private List<Movie> ImportMoviesToMongoScheme()
        {
            LogHelper.Log("Importing MOVIES", nameof(SqlService));

            var movieIDs = RunQuery("SELECT MovieID FROM MOVIES ORDER BY 1").Select(int.Parse).ToList();

            var movies = new List<Movie>();

            foreach (var movieId in movieIDs)
            {
                var movieRowString = RunQuery($"SELECT Title, Age, MediaType, Runtime, MovieID FROM MOVIES WHERE MovieID = {movieId} ORDER BY MovieID").First();
                var ratingRowStrings = RunQuery($"SELECT RatingSite, Rating FROM MOVIE_RATINGS WHERE MovieID = {movieId}");
                var countryRowStrings = RunQuery($@"SELECT C.CountryCode, C.Country, MovieID FROM MOVIE_IN_COUNTRIES MC
                                                    INNER JOIN COUNTRIES C ON C.CountryCode = MC.CountryCode
                                                    WHERE MovieID = {movieId}");
                var platformStrings = RunQuery($"SELECT Platform FROM MOVIE_ON_PLATFORMS WHERE MovieId = {movieId}");

                movies.Add(movieRowString.ToMovie(ratingRowStrings, countryRowStrings, platformStrings));
            }

            LogHelper.Log("Import complete", nameof(SqlService));

            return movies;
        }

        private List<User> ImportUsersToMongoScheme()
        {
            LogHelper.Log("Importing USERS", nameof(SqlService));

            var userIDs = RunQuery("SELECT UserID FROM USERS ORDER BY 1").Select(int.Parse).ToList();

            var users = new List<User>();

            foreach (var userId in userIDs)
            {
                var userRowString = RunQuery($"SELECT FullName, Email, BirthDate, CountryCode, Sex, UserID FROM USERS WHERE UserID = {userId}").First();
                var favoriteMovieRowStrings = RunQuery(@$"  SELECT Title, Age, MediaType, Runtime, M.MovieID
                                                            FROM MOVIES M
                                                            INNER JOIN FAVORITE_MOVIES_PER_USER F
	                                                            ON M.MovieID = F.MovieID
                                                            WHERE UserId = {userId}
                                                            ORDER BY M.MovieID");
                var platformStrings = RunQuery($"SELECT Platform FROM PLATFORM_USERS WHERE UserID = {userId}");
                var mediaTypeStrings = RunQuery($"SELECT MediaType FROM USER_MEDIA_TYPES WHERE UserID = {userId}");

                users.Add(userRowString.ToUser(favoriteMovieRowStrings, platformStrings, mediaTypeStrings));
            }

            LogHelper.Log("Import complete", nameof(SqlService));

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
