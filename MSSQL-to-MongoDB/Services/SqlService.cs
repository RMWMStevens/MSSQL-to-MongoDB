﻿using MSSQL_to_MongoDB.Extensions;
using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Models;
using MSSQL_to_MongoDB.Models.MSSQL;
using System;

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

        public ActionResult<MSSQL> ImportDatabase()
        {
            try
            {
                var database = new MSSQL()
                {
                    Countries = SqlHelper.RunQuery("SELECT * FROM COUNTRIES ORDER BY 1").ToCountries(),
                    FavoriteMoviesPerUser = SqlHelper.RunQuery("SELECT * FROM FAVORITE_MOVIES_PER_USER ORDER BY 1").ToFavoriteMoviesPerUser(),
                    MovieInCountries = SqlHelper.RunQuery("SELECT * FROM MOVIE_IN_COUNTRIES ORDER BY 1").ToMovieInCountries(),
                    MovieOnPlatforms = SqlHelper.RunQuery("SELECT * FROM MOVIE_ON_PLATFORMS ORDER BY 1").ToMovieOnPlatforms(),
                    MovieRatings = SqlHelper.RunQuery("SELECT * FROM MOVIE_RATINGS ORDER BY 1").ToMovieRatings(),
                    Movies = SqlHelper.RunQuery("SELECT * FROM MOVIES ORDER BY 1").ToMovies(),
                    PlatformUsers = SqlHelper.RunQuery("SELECT * FROM PLATFORM_USERS ORDER BY 1").ToPlatformUsers(),
                    UserMediaTypes = SqlHelper.RunQuery("SELECT * FROM USER_MEDIA_TYPES ORDER BY 1").ToUserMediaTypes(),
                    Users = SqlHelper.RunQuery("SELECT * FROM USERS").ToUsers(),
                };

                return ActionResultHelper.CreateSuccessResult(database);
            }
            catch (Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<MSSQL>(ex);
            }
        }
    }
}
