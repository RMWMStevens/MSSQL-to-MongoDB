using MongoDB.Driver;
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
                Insert(Collections.MOVIES, mongoDb.Movies);
                Insert(Collections.USERS, mongoDb.Users);

                var movieRefs = GetMovieReferences(mongoDb.Movies);
                DropCollection(Collections.MOVIES);
                Insert(Collections.MOVIES, movieRefs);

                var userRefs = GetUserReferences(mongoDb.Users);
                DropCollection(Collections.USERS);
                Insert(Collections.USERS, userRefs);

                return new ActionResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<string>(ex);
            }
        }

        private List<Movie_REF> GetMovieReferences(List<Movie> movies)
        {
            var movieRefs = new List<Movie_REF>();

            Console.WriteLine($"MongoService - Getting MOVIE references");

            foreach (var movie in movies)
            {
                movieRefs.Add(GetMovieReferences(movie));
            }

            return movieRefs;
        }

        private Movie_REF GetMovieReferences(Movie movie)
        {
            var database = GetDatabase();
            var countriesCollection = database.GetCollection<Country>(Collections.COUNTRIES.ToString());

            var movieFilterDef = new FilterDefinitionBuilder<Country>();
            var movieFilter = movieFilterDef.In(x => x.CountryCode, movie.ReleasedInCountries.Select(f => f.CountryCode));
            var releasedInCountries = countriesCollection.Find(movieFilter).ToList();

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

        private List<User_REF> GetUserReferences(List<User> users)
        {
            var userRefs = new List<User_REF>();

            Console.WriteLine($"MongoService - Getting USER references");

            foreach (var user in users)
            {
                userRefs.Add(GetUserReferences(user));
            }

            return userRefs;
        }

        private User_REF GetUserReferences(User user)
        {
            var database = GetDatabase();
            var countriesCollection = database.GetCollection<Country>(Collections.COUNTRIES.ToString());
            var moviesCollection = database.GetCollection<Movie>(Collections.MOVIES.ToString());

            var country = countriesCollection.Find(c => c.CountryCode == user.CountryCode).FirstOrDefault();

            var movieFilterDef = new FilterDefinitionBuilder<Movie>();
            var movieFilter = movieFilterDef.In(x => x.MovieID, user.Favorites.Select(f => f.MovieID));
            var favorites = moviesCollection.Find(movieFilter).ToList();

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
