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
    public class MongoService : BaseDbService
    {
        private const string databaseName = "What2Watch";

        public MongoService()
        {
            system = Models.Enums.DatabaseSystem.MongoDB;
        }

        public override string GetExampleConnectionStringFormat()
        {
            return @"mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb";
        }

        public async Task<ActionResult> ExportAsync(MongoDb mongoDb)
        {
            try
            {
                await DropCollectionsAsync();

                await Task.WhenAll(
                    InsertAsync(MongoCollection.Movies, mongoDb.Movies),
                    InsertAsync(MongoCollection.Users, mongoDb.Users)
                );

                var userRefs = await GetUserReferencesAsync(mongoDb.Users);
                await DropCollection(MongoCollection.Users);
                await InsertAsync(MongoCollection.Users, userRefs);

                return new ActionResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<string>(ex);
            }
        }

        private async Task<IEnumerable<UserRef>> GetUserReferencesAsync(IEnumerable<User> users)
        {
            LogHelper.Log("Getting user references", nameof(MongoService));

            var userRefTasks = new List<Task<UserRef>>();

            foreach (var user in users)
            {
                userRefTasks.Add(GetUserReferencesAsync(user));
            }

            return await Task.WhenAll(userRefTasks);
        }

        private async Task<UserRef> GetUserReferencesAsync(User user)
        {
            var database = GetDatabase();
            var moviesCollection = database.GetCollection<Movie>(MongoCollection.Movies.ToString());

            var movieFilterDef = new FilterDefinitionBuilder<Movie>();
            var movieFilter = movieFilterDef.In(x => x.MovieID, user.Favorites.Select(f => f.MovieID));

            var favorites = await moviesCollection.Find(movieFilter).ToListAsync();

            return new UserRef
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                BirthDate = user.BirthDate,
                MediaTypes = user.MediaTypes,
                Sex = user.Sex,
                Country = user.Country,
                Platforms = user.Platforms,
                Favorites = favorites.Select(x => x.Id),
            };
        }

        private async Task InsertAsync<T>(MongoCollection collectionName, IEnumerable<T> list)
        {
            LogHelper.Log($"Exporting {collectionName}", nameof(MongoService));
            var database = GetDatabase();
            var collection = database.GetCollection<T>(collectionName.ToString());
            await collection.InsertManyAsync(list);
        }

        private async Task DropCollectionsAsync()
        {
            LogHelper.Log("Dropping collections", nameof(MongoService));
            var collections = (MongoCollection[])Enum.GetValues(typeof(MongoCollection));

            var dropTasks = new List<Task>();
            foreach (var collection in collections)
            {
                dropTasks.Add(DropCollection(collection));
            }
            await Task.WhenAll(dropTasks);
        }

        private async Task DropCollection(MongoCollection collectionName)
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
