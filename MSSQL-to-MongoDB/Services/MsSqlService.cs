using MSSQL_to_MongoDB.Extensions;
using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Models;
using MSSQL_to_MongoDB.Models.Enums;
using MSSQL_to_MongoDB.Models.MongoDB;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MSSQL_to_MongoDB.Services
{
    public class MsSqlService : BaseDbService
    {
        public MsSqlService()
        {
            system = DatabaseSystem.MSSQL;
        }

        public override string GetExampleConnectionStringFormat()
        {
            var sqlAuth = @"Data Source=COMPUTERNAME;Initial Catalog=DATABASENAME;User ID=USERNAME;Password=PASSWORD";
            var winAuth = @"Data Source=COMPUTERNAME;Initial Catalog=DATABASENAME;Integrated Security=SSPI;";

            return $"For SQL Authentication: \n{sqlAuth}\nFor Windows Authentication (leave 'SSPI' as is): \n{winAuth}";
        }

        public async Task<ActionResult<MongoDb>> ImportToMongoSchemaAsync()
        {
            try
            {
                var moviesTask = ImportMoviesToMongoSchemaAsync();
                var usersTask = ImportUsersToMongoSchemaAsync();

                await Task.WhenAll(moviesTask, usersTask);

                var mongoDb = new MongoDb
                {
                    Movies = await moviesTask,
                    Users = await usersTask,
                };
                return ActionResultHelper.CreateSuccessResult(mongoDb);
            }
            catch (Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<MongoDb>(ex);
            }
        }

        private async Task<IEnumerable<Movie>> ImportMoviesToMongoSchemaAsync()
        {
            LogHelper.Log("Importing movies", nameof(MsSqlService));

            var movieIDs = (await RunQueryAsync("SELECT MovieID FROM MOVIES ORDER BY 1")).Select(int.Parse);

            var movieTasks = new List<Task<Movie>>();

            foreach (var movieId in movieIDs)
            {
                movieTasks.Add(ImportMovieToMongoSchemaAsync(movieId));
            }

            LogHelper.Log("Importing movies complete", nameof(MsSqlService));

            return await Task.WhenAll(movieTasks);
        }

        private async Task<Movie> ImportMovieToMongoSchemaAsync(int movieId)
        {
            var rowStringTasks = new List<Task<IEnumerable<string>>>();

            var movieQuery = $"SELECT Title, Age, MediaType, Runtime, MovieID FROM MOVIES WHERE MovieID = {movieId} ORDER BY MovieID";
            rowStringTasks.Add(RunQueryAsync(movieQuery));

            var ratingQuery = $"SELECT RatingSite, Rating FROM MOVIE_RATINGS WHERE MovieID = {movieId}";
            rowStringTasks.Add(RunQueryAsync(ratingQuery));

            var countryQuery = $@"SELECT C.CountryCode, C.Country FROM MOVIE_IN_COUNTRIES MC
                                INNER JOIN COUNTRIES C ON C.CountryCode = MC.CountryCode
                                WHERE MovieID = {movieId}";
            rowStringTasks.Add(RunQueryAsync(countryQuery));

            var platformQuery = $"SELECT Platform FROM MOVIE_ON_PLATFORMS WHERE MovieId = {movieId}";
            rowStringTasks.Add(RunQueryAsync(platformQuery));

            var rowStrings = await Task.WhenAll(rowStringTasks);
            return rowStrings[0].First().ToMovie(rowStrings[1], rowStrings[2], rowStrings[3]);
        }

        private async Task<IEnumerable<User>> ImportUsersToMongoSchemaAsync()
        {
            LogHelper.Log("Importing users", nameof(MsSqlService));

            var userIDs = (await RunQueryAsync("SELECT UserID FROM USERS ORDER BY 1")).Select(int.Parse);

            var userTasks = new List<Task<User>>();

            foreach (var userId in userIDs)
            {
                userTasks.Add(ImportUserToMongoSchemaAsync(userId));
            }

            LogHelper.Log("Importing users complete", nameof(MsSqlService));

            return await Task.WhenAll(userTasks);
        }

        private async Task<User> ImportUserToMongoSchemaAsync(int userId)
        {
            var rowStringTasks = new List<Task<IEnumerable<string>>>();

            var userQuery = @$"SELECT FullName, Email, BirthDate, Sex, UserID FROM USERS U WHERE UserID = {userId}";
            rowStringTasks.Add(RunQueryAsync(userQuery));

            var favoriteMovieQuery = @$"SELECT Title, Age, MediaType, Runtime, M.MovieID
                                        FROM MOVIES M
                                        INNER JOIN FAVORITE_MOVIES_PER_USER F
	                                        ON M.MovieID = F.MovieID
                                        WHERE UserId = {userId}
                                        ORDER BY M.MovieID";
            rowStringTasks.Add(RunQueryAsync(favoriteMovieQuery));

            var platformQuery = $"SELECT Platform FROM PLATFORM_USERS WHERE UserID = {userId}";
            rowStringTasks.Add(RunQueryAsync(platformQuery));

            var mediaTypeQuery = $"SELECT MediaType FROM USER_MEDIA_TYPES WHERE UserID = {userId}";
            rowStringTasks.Add(RunQueryAsync(mediaTypeQuery));

            var countryQuery = $@"SELECT C.CountryCode, Country
                                FROM COUNTRIES C
                                INNER JOIN USERS U
	                                ON C.CountryCode = U.CountryCode
                                WHERE UserID = {userId}";
            rowStringTasks.Add(RunQueryAsync(countryQuery));

            var rowStrings = await Task.WhenAll(rowStringTasks);
            return rowStrings[0].First().ToUser(rowStrings[1], rowStrings[2], rowStrings[3], rowStrings[4]);
        }

        public async Task<IEnumerable<string>> RunQueryAsync(string sqlQuery)
        {
            var sqlConnection = new SqlConnection(connectionString);
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

            await sqlConnection.CloseAsync();
            return rows;
        }
    }
}
