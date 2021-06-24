using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Models.MongoDB.References
{
    public class MONGO_DB_REF
    {
        public List<Movie_REF> Movies { get; set; }
        public List<User_REF> Users { get; set; }
        public List<Country> Countries { get; set; }
        public List<Platform_REF> Platforms { get; set; }
    }
}
