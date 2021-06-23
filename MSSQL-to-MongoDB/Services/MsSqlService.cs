using MSSQL_to_MongoDB.Extensions;
using MSSQL_to_MongoDB.Models.MSSQL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MSSQL_to_MongoDB.Services
{
    public class MsSqlService : DatabaseService
    {
        public MsSqlService()
        {
            system = Models.Enums.DatabaseSystem.MSSQL;
        }

        public override string GetExampleFormat()
        {
            var sqlAuth = @"Data Source=COMPUTERNAME;Initial Catalog=DATABASENAME;User ID=USERNAME;Password=PASSWORD";
            var winAuth = @"Data Source=COMPUTERNAME;Initial Catalog=DATABASENAME;Integrated Security=SSPI;";

            return $"For SQL Authentication: \n{sqlAuth}\nFor Windows Authentication: \n{winAuth}";
        }

        public MSSQL Import()
        {
            var database = new MSSQL()
            {
                Countries = RunQuery("SELECT * FROM COUNTRIES").ToCountries(),
                FavoriteMoviesPerUser = RunQuery("SELECT * FROM FAVORITE_MOVIES_PER_USER").ToFavoriteMoviesPerUser(),
                MovieInCountries = RunQuery("SELECT * FROM MOVIE_IN_COUNTRIES").ToMovieInCountries(),
                MovieOnPlatforms = RunQuery("SELECT * FROM MOVIE_ON_PLATFORMS").ToMovieOnPlatforms(),
                MovieRatings = RunQuery("SELECT * FROM MOVIE_RATINGS").ToMovieRatings(),
                Movies = RunQuery("SELECT * FROM MOVIES").ToMovies(),
                PlatformUsers = RunQuery("SELECT * FROM PLATFORM_USERS").ToPlatformUsers(),
                UserMediaTypes = RunQuery("SELECT * FROM USER_MEDIA_TYPES").ToUserMediaTypes(),
                Users = RunQuery("SELECT * FROM USERS").ToUsers(),
            };

            return database;
        }

        public List<string> RunQuery(string sqlQuery)
        {
            try
            {
                var sqlConnection = new SqlConnection(connectionString);
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}
