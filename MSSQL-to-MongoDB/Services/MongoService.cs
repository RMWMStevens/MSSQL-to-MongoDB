using MongoDB.Bson;
using MongoDB.Driver;
using MSSQL_to_MongoDB.Extensions;
using MSSQL_to_MongoDB.Helpers;
using MSSQL_to_MongoDB.Models;
using MSSQL_to_MongoDB.Models.MongoDB;
using MSSQL_to_MongoDB.Models.MongoDB.Enums;
using MSSQL_to_MongoDB.Models.MongoDB.References;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public ActionResult Export(MONGO_DB mongoDb)
        {
            try
            {
                DropCollections();

                Insert(Collections.COUNTRIES, mongoDb.Countries);
                Insert(Collections.PLATFORMS, mongoDb.Platforms);
                Insert(Collections.MOVIES, mongoDb.Movies);
                Insert(Collections.USERS, mongoDb.Users);

                var userRefs = UpdateUserReferences(mongoDb.Users);
                DropCollection(Collections.USERS);
                Insert(Collections.USERS, userRefs);

                return new ActionResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<string>(ex);
            }
        }

        //public ActionResult ExportReferences(MONGO_DB_REF mongoDbRef)
        //{
        //    try
        //    {
        //        // Insert(Collections.COLLECTION, mongoDb.ATTRIBUTE);

        //        return new ActionResult { IsSuccess = true };
        //    }
        //    catch (Exception ex)
        //    {
        //        return ActionResultHelper.CreateErrorResult<string>(ex);
        //    }
        //}

        private List<User_REF> UpdateUserReferences(List<User> users)
        {
            var userRefs = new List<User_REF>();

            foreach (var user in users)
            {
                userRefs.Add(GetUserReferences(user));
            }

            return userRefs;
        }

        private User_REF GetUserReferences(User user)
        {
            var database = GetDatabase();
            var usersCollection = database.GetCollection<User>(Collections.USERS.ToString());
            var countriesCollection = database.GetCollection<Country>(Collections.COUNTRIES.ToString());
            var moviesCollection = database.GetCollection<Movie>(Collections.MOVIES.ToString());
            var platformsCollection = database.GetCollection<Platform>(Collections.PLATFORMS.ToString());

            var country = countriesCollection.Find(c => c.CountryCode == user.CountryCode).FirstOrDefault();
            
            var movieFilterDef = new FilterDefinitionBuilder<Movie>();
            var movieFilter = movieFilterDef.In(x => x.MovieID, user.Favorites.Select(f => f.MovieID));
            var favorites = moviesCollection.Find(movieFilter).ToList();

            var platformFilterDef = new FilterDefinitionBuilder<Platform>();
            var platformFilter = platformFilterDef.In(x => x.PlatformName, user.Platforms.Select(p => p.PlatformName));
            var platforms = platformsCollection.Find(platformFilter).ToList();

            return new User_REF
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                BirthDate = user.BirthDate,
                MediaTypes = user.MediaTypes,
                Sex = user.Sex,
                Country = country.Id,
                Favorites = favorites.Select(x => x.Id).ToList(),
                Platforms = platforms.Select(x => x.Id).ToList(),
            };
        }

        private void Insert<T>(Collections collectionName, List<T> list)
        {
            Console.WriteLine($"MongoService - Exporting {collectionName}");
            var database = GetDatabase();
            var collection = database.GetCollection<T>(collectionName.ToString());
            collection.InsertMany(list);
        }

        private void DropCollections()
        {
            Console.WriteLine("MongoService - Dropping collections");
            var collections = (Collections[])Enum.GetValues(typeof(Collections));
            foreach (var collection in collections)
            {
                DropCollection(collection);
            }
        }

        private void DropCollection(Collections collectionName)
        {
            var database = GetDatabase();
            database.DropCollection(collectionName.ToString());
        }

        private IMongoDatabase GetDatabase()
        {
            var mongoClient = new MongoClient(connectionString);
            return mongoClient.GetDatabase(databaseName);
        }
    }
}
