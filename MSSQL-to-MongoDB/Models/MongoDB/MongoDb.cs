using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Models.MongoDB
{
    public class MongoDb
    {
        public IEnumerable<Movie> Movies { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}
