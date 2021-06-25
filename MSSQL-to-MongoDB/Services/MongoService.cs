using MongoDB.Driver;
using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Models;
using MSSQL_to_MongoDB.Models.MongoDB;
using MSSQL_to_MongoDB.Models.MongoDB.Enums;
using MSSQL_to_MongoDB.Models.MongoDB.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSSQL_to_MongoDB.Services
{
    public class MongoService : DatabaseService
    {
        private const string databaseName = "What2Watch";

        public MongoService()
        {
            system = Models.Enums.DatabaseSystem.MongoDB;
        }

        public override string GetExampleFormat()
        {
            return @"mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb";
        }

        public async Task<ActionResult> Export(MONGO_DB mongoDb)
        {
            try
            {
                await DropCollections();
                
                await Insert (Collections.COUNTRIES, mongoDb.Countries);
                await Insert(Collections.MOVIES, mongoDb.Movies);
                await Insert(Collections.USERS, mongoDb.Users);

                var movieRefs = await GetMovieReferences(mongoDb.Movies);
                await DropCollection(Collections.MOVIES);
                await Insert(Collections.MOVIES, movieRefs);

                var userRefs = await GetUserReferences(mongoDb.Users);
                await DropCollection(Collections.USERS);
                await Insert(Collections.USERS, userRefs);

                return new ActionResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<string>(ex);
            }
        }

        private async Task<List<Movie_REF>> GetMovieReferences(List<Movie> movies)
        {
            LogHelper.Log("Getting MOVIE references", nameof(MongoService));

            var movieRefTasks = new List<Task<Movie_REF>>(); 

            foreach (var movie in movies)
            {
                movieRefTasks.Add(GetMovieReferences(movie));
            }

            return (await Task.WhenAll(movieRefTasks)).ToList();
        }

        private async Task<Movie_REF> GetMovieReferences(Movie movie)
        {
            var database = GetDatabase();
            var countriesCollection = database.GetCollection<Country>(Collections.COUNTRIES.ToString());

            var movieFilterDef = new FilterDefinitionBuilder<Country>();
            var movieFilter = movieFilterDef.In(x => x.CountryCode, movie.ReleasedInCountries.Select(f => f.CountryCode));
            var releasedInCountries = await countriesCollection.Find(movieFilter).ToListAsync();

            return new Movie_REF
            {
                Id = movie.Id,
                Title = movie.Title,
                Age = movie.Age,
                MediaType = movie.MediaType,
                Runtime = movie.Runtime,
                Ratings = movie.Ratings,
                Platforms = movie.Platforms,
                ReleasedInCountries = releasedInCountries.Select(x => x.Id).ToList()
            };
        }

        private async Task<List<User_REF>> GetUserReferences(List<User> users)
        {
            LogHelper.Log("Getting USER references", nameof(MongoService));

            var userRefTasks = new List<Task<User_REF>>();

            foreach (var user in users)
            {
                userRefTasks.Add(GetUserReferences(user));
            }

            return (await Task.WhenAll(userRefTasks)).ToList();
        }

        private async Task<User_REF> GetUserReferences(User user)
        {
            var database = GetDatabase();
            var countriesCollection = database.GetCollection<Country>(Collections.COUNTRIES.ToString());
            var moviesCollection = database.GetCollection<Movie>(Collections.MOVIES.ToString());

            var movieFilterDef = new FilterDefinitionBuilder<Movie>();
            var movieFilter = movieFilterDef.In(x => x.MovieID, user.Favorites.Select(f => f.MovieID));
            var favorites = await moviesCollection.Find(movieFilter).ToListAsync();

            var country = await countriesCollection.Find(c => c.CountryCode == user.CountryCode).FirstOrDefaultAsync();

            return new User_REF
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                BirthDate = user.BirthDate,
                MediaTypes = user.MediaTypes,
                Sex = user.Sex,
                Country = country.Id,
                Platforms = user.Platforms,
                Favorites = favorites.Select(x => x.Id).ToList(),
            };
        }

        private async Task Insert<T>(Collections collectionName, List<T> list)
        {
            LogHelper.Log($"Exporting {collectionName}", nameof(MongoService));
            var database = GetDatabase();
            var collection = database.GetCollection<T>(collectionName.ToString());
            await collection.InsertManyAsync(list);
        }

        private async Task DropCollections()
        {
            LogHelper.Log("Dropping collections", nameof(MongoService));
            var collections = (Collections[])Enum.GetValues(typeof(Collections));

            var dropTasks = new List<Task>();
            foreach (var collection in collections)
            {
                dropTasks.Add(DropCollection(collection));
            }
            await Task.WhenAll(dropTasks);
        }

        private async Task DropCollection(Collections collectionName)
        {
            var database = GetDatabase();
            await database.DropCollectionAsync(collectionName.ToString());
        }

        private IMongoDatabase GetDatabase()
        {
            var mongoClient = new MongoClient(connectionString);
            return mongoClient.GetDatabase(databaseName);
        }
    }
}
