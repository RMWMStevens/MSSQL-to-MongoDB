using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Models.MongoDB
{
    public class Platform
    {
        public string PlatformName { get; set; }
        public List<Movie> Movies { get; set; }
    }
}
