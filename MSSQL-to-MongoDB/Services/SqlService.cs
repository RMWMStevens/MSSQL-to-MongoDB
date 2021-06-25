using MSSQL_to_MongoDB.Extensions;
using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Models;
using MSSQL_to_MongoDB.Models.MongoDB;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<ActionResult<MONGO_DB>> ImportToMongoSchema()
        {
            try
            {
                var mongoDb = new MONGO_DB
                {
                    Countries = await ImportCountriesToMongoSchema(),
                    Movies = await ImportMoviesToMongoSchema(),
                    Users = await ImportUsersToMongoSchema(),
                };
                return ActionResultHelper.CreateSuccessResult(mongoDb);
            }
            catch (Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<MONGO_DB>(ex);
            }
        }

        private async Task<List<Country>> ImportCountriesToMongoSchema()
        {
            LogHelper.Log("Importing COUNTRIES", nameof(SqlService));

            var countryRowStrings = await RunQuery("SELECT CountryCode, Country FROM COUNTRIES ORDER BY 1");

            LogHelper.Log("Import complete", nameof(SqlService));

            return countryRowStrings.ToCountries();
        }

        private async Task<List<Movie>> ImportMoviesToMongoSchema()
        {
            LogHelper.Log("Importing MOVIES", nameof(SqlService));

            var movieIDs = (await RunQuery("SELECT MovieID FROM MOVIES ORDER BY 1")).Select(int.Parse).ToList();

            var movieTasks = new List<Task<Movie>>();

            foreach (var movieId in movieIDs)
            {
                movieTasks.Add(ImportMovieToMongoSchema(movieId));
            }

            LogHelper.Log("Import complete", nameof(SqlService));

            return (await Task.WhenAll(movieTasks)).ToList();
        }

        private async Task<Movie> ImportMovieToMongoSchema(int movieId)
        {
            var stringTasks = new List<Task<List<string>>>();

            var movieQuery = $"SELECT Title, Age, MediaType, Runtime, MovieID FROM MOVIES WHERE MovieID = {movieId} ORDER BY MovieID";
            stringTasks.Add(RunQuery(movieQuery));

            var ratingQuery = $"SELECT RatingSite, Rating FROM MOVIE_RATINGS WHERE MovieID = {movieId}";
            stringTasks.Add(RunQuery(ratingQuery));

            var countrySql = $@"SELECT C.CountryCode, C.Country, MovieID FROM MOVIE_IN_COUNTRIES MC
                                INNER JOIN COUNTRIES C ON C.CountryCode = MC.CountryCode
                                WHERE MovieID = {movieId}";
            stringTasks.Add(RunQuery(countrySql));

            var platformSql = $"SELECT Platform FROM MOVIE_ON_PLATFORMS WHERE MovieId = {movieId}";
            stringTasks.Add(RunQuery(platformSql));

            var rowStrings = await Task.WhenAll(stringTasks);
            return rowStrings[0].First().ToMovie(rowStrings[1], rowStrings[2], rowStrings[3]);
        }

        private async Task<List<User>> ImportUsersToMongoSchema()
        {
            LogHelper.Log("Importing USERS", nameof(SqlService));

            var userIDs = (await RunQuery("SELECT UserID FROM USERS ORDER BY 1")).Select(int.Parse).ToList();

            var userTasks = new List<Task<User>>();

            foreach (var userId in userIDs)
            {
                userTasks.Add(ImportUserToMongoSchema(userId));
            }

            LogHelper.Log("Import complete", nameof(SqlService));

            return (await Task.WhenAll(userTasks)).ToList();
        }

        private async Task<User> ImportUserToMongoSchema(int userId)
        {
            var stringTasks = new List<Task<List<string>>>();

            var userQuery = $"SELECT FullName, Email, BirthDate, CountryCode, Sex, UserID FROM USERS WHERE UserID = {userId}";
            stringTasks.Add(RunQuery(userQuery));

            var favoriteMovieQuery = @$"SELECT Title, Age, MediaType, Runtime, M.MovieID
                                        FROM MOVIES M
                                        INNER JOIN FAVORITE_MOVIES_PER_USER F
	                                        ON M.MovieID = F.MovieID
                                        WHERE UserId = {userId}
                                        ORDER BY M.MovieID";
            stringTasks.Add(RunQuery(favoriteMovieQuery));

            var platformQuery = $"SELECT Platform FROM PLATFORM_USERS WHERE UserID = {userId}";
            stringTasks.Add(RunQuery(platformQuery));

            var mediaTypeQuery = $"SELECT MediaType FROM USER_MEDIA_TYPES WHERE UserID = {userId}";
            stringTasks.Add(RunQuery(mediaTypeQuery));

            var rowStrings = await Task.WhenAll(stringTasks);
            return rowStrings[0].First().ToUser(rowStrings[1], rowStrings[2], rowStrings[3]);
        }

        public async Task<List<string>> RunQuery(string sqlQuery)
        {
            var sqlConnection = new SqlConnection(ConnectionString);
            await sqlConnection.OpenAsync();

            var command = new SqlCommand(sqlQuery, sqlConnection);
            var dataReader = await command.ExecuteReaderAsync();

            var rows = new List<string>();

            while (await dataReader.ReadAsync())
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
