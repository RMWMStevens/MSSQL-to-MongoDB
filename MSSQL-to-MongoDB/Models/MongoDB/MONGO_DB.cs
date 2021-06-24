using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Models.MongoDB
{
    public class MONGO_DB
    {
        public List<Movie> Movies { get; set; }
        public List<User> Users { get; set; }
        public List<Country> Countries { get; set; }
        public List<Platform> Platforms { get; set; }
    }
}
